# Developer Guide
## Installation: Nuget
Install the latest version of nuget package using the command below.
```
NuGet\Install-Package Parsley.Net
```

## Implementation: Using Parsley.Net
### Step 1. Create Aggregated Contract
`Aggregated Contract` is the resultant response from all the aggregated apis. To create aggregated contract derive the class from `IContract` interface.

Example.
```
 public class Customer : IContract
 {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Contacts Communication { get; set; }
        public Order[] Orders { get; set; }
}
```
### Step 2. Create Api Aggregated
`Api Aggregate` is the composition of apis configured to obtain an aggregated data populated contract. To create an Api Aggregate derive from `ApiAggregate<TContract>` class where `TContract` is an implementation of IContract (ie. Agggregated Contract).

Example.
```
internal class CustomerAggregate : ApiAggregate<Customer>
{
        /// <summary>
        /// Constructs the api aggregate with web apis and result transformers to map data to aggregated contract.
        /// </summary>
        /// <returns>Mappings</returns>
        public override IEnumerable<Mapping<Customer, IApiResult>> Construct()
        {
            return CreateAggregate.For<Customer>()
                .Map<CustomerApi, CustomerTransform>(With.Name("customer"),
                 customer => customer.Dependents
                    .Map<CommunicationApi, CommunicationTransform>(With.Name("customer.communication"))
                    .Map<OrdersApi, OrdersTransform>(With.Name("customer.orders"),
                        customerOrders => customerOrders.Dependents
                            .Map<OrderItemsApi, OrderItemsTransform>(With.Name("customer.orders.items")))
                ).Create();
        }
}
```
`Api Aggregate` comprises of apis configured in hierarchical nested graph with each api having an associated result transformers. The result from the api is fed to the associated result transformer to map data to the aggregated contract.

### 2.1 Api & Transformer Pair
Every `Api` type in the `ApiAggregate` definition should have a complementing `Transformer` type.
- You need to assign a `name` to the `api/transformer` pair. See below rules for api naming convention.

Rules:
* You could nest api/transformer pairs in a `parent/child` hierarchy. In a given parent/child hierarchy, the output of the parent api will serve as the input to the nested api to resolve its api endpoint.
* The api/transformer mappings can be `nested` to `5` levels of dependency.
* By convention, The api name should be `dot` separated string with a dot for every nested level. The name should includes all parent names separated by a dot for a given hierarchy.
ie. For a 3 level dependency api mapping the name should be like `customer.orders.items`

Example.
> Below is the snippet from `CustomerAggregate` definition for parent/child relationship between Customer & Communication apis. The api response from CustomerApi is the input to CommunicationApi for resolving its endpoint url.
```
   .Map<CustomerApi, CustomerTransform>(With.Name("customer"), -- Parent mapping with name
           customer => customer.Dependents
              .Map<CommunicationApi, CommunicationTransform>(With.Name("customer.communication")) -- nested mapping with dot separated name
```
#### i. Web Api Class
The purpose of a api class is to execute the web api with api engine to fetch the response.

As mentioned previously, You can configure an api in `Parent` or `Child` (nested) mode in a hierarchical graph.

To create `Web Api` defined as `parent` or `nested` api, you need to implement from `WebApi<TResult>` class,
where `TResult` is `IApiResult` interface (or `ApiResult` base class) implementation and is the result that will be returned from executing the api.

Upon creating the web api class, you need to provide `GetUrl()` method implementation to return `Uri` instance.
* Implement the `GetUrl(IRequestContext context, IApiResult parentApiResult)` method to return the constructed endpoint using given parameters of the method.
* For `Parent Api`, only `IRequestContext` context parameter is passed to GetUrl() method to resolve the Url endpoint. 
* For `Nested Api`, api result parameter (ie. `IApiResult` parentApiResult) from the parent api is additionally passed in to GetUrl() method along with IRequestContext context parameter.
* Optionally, override `GetRequestHeaders()` method to provide a dictionary of `outgoing headers` for the api request.
* Optionally, override `GetResponseHeaders()` method to provide any list of `incoming headers` from the api response.
* `IApiResult` implementation exposes `Headers` property for subscribed `response headers` received as part of the api response.

`Important:`
- The api `endpoint` needs to be resolved before executing the api with `ApiEngine`.
- `IApiResult` parentApiResult parameter is null for apis configured in parent mode.

Examples.

> See example `CustomerApi` implemented to be configured and run in parent mode. 
 ```
public class CustomerApi : WebApi<CustomerResult>
{
    public CustomerApi() : base(Endpoints.BaseAddress)
    {
    }

    // Override to construct the api endpoint.
    protected override Uri GetUrl(IRequestContext context, IApiResult parentApiResult)
    {
        // Executes as root or level 1 api. parentApiResult should be null.
        var customerContext = (CustomerContext)context;

        return new Uri(string.Format(Endpoints.Customer, customerContext.CustomerId));
    }

    // Override to pass custom outgoing headers with the api request.
    protected override IDictionary<string, string> GetRequestHeaders()
    {
        return new Dictionary<string, string>
        {
            { "x-meta-branch-code", "Geneva" }
        };
    }

    // Override to get custom incoming headers with the api response.
    protected override IEnumerable<string> GetResponseHeaders()
    {
        return new[] { "x-meta-branch-code" };
    }
}
```

> See example `CommunicationApi` implemented to be configured and run as nested api to customer api below.
```
internal class CommunicationApi : WebApi<CommunicationResult>
    {
        public CommunicationApi() : base(Endpoints.BaseAddress)
        {
        }

        protected override Uri GetUrl(IRequestContext context, IApiResult parentApiResult)
        {
            var customer = (CustomerResult)parentApiResult;
            return new Uri(string.Format(Endpoints.Communication, customer.Id));
        }
    }
```
##### ii. Result Transformer Class
The purpose of the transformer class is to map the response fetched by the linked api to the aggregated contract.

To define a transformer class, you need to implement `ResultTransformer<TResult, TContract>` class.
- where TContract is Aggregated Contract implementing `IContract`. eg. Customer. 
- where TResult is Api Result from associated Query. It is an implementation of `IApiResult` interface. 

Example.

> `CustomerTransformer` is implemented to map `CustomerResult` recevied from CustomerApi to `Customer` Aggregated Contract.

```
public class CustomerTransform : ResultTransformer<CustomerResult, Customer>
    {
        public override void Transform(CustomerResult apiResult, Customer contract)
        {
            var customer = contract ?? new Customer();
            customer.Id = apiResult.Id;
            customer.Name = apiResult.Name;
            customer.Code = apiResult.Code;
        }
    }
```

### Step 3. Parsley.Net Setup
`Parsley.Net` needs to setup with required dependencies.

#### i. IoC Registrations
With ServiceCollection, you need to register the below dependencies.
```
    // Register core services
    services.AddTransient(typeof(IApiBuilder<>), typeof(ApiBuilder<>));
    services.AddTransient(typeof(IContractBuilder<>), typeof(ContractBuilder<>));
    services.AddTransient(typeof(IParsley.Net<>), typeof(Parsley.Net<>));
    services.AddTransient<IApiExecutor, ApiExecutor>();
     services.AddTransient<IApiEnginne, ApiEngine>();

    // Register instance of IApiNameMatcher.
    services.AddTransient(c => new StringColonSeparatedMatcher());

    // Enable logging
    services.AddLogging();

    // Enable HttpClient
     services.AddHttpClient();

    // Register api aggregate definitions. eg CustomerAggregate
    services.AddTransient<IApiAggregate<Customer>, CustomerAggregate>();
```
#### ii. With Fluent Registration Api
You could also acheieve the above registrations using fluent registration below.
```
    // Enable logging
    services.AddLogging();

    // Enable HttpClient
     services.AddHttpClient();

    // Fluent registration.
     services.UseParsley.Net()
             .AddApiAggregate<Customer>(new CustomerAggregate());
```
Note: Above examples to enable HttpClient is basic. However, you could additionally implement circuit breaking & retry policies.
>Please see [`Circuit Breaker`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern) pattern for more details.


### Step 4. Use IParsley.Net<`TContract`>

#### i. IApiAggrgator (DI)
To use Api aggregator, Inject IApiAggrgator<TContract> where TContract is IContract, using constructor & property injection method or explicity Resolve using service provider

Example. `IServiceProvider.GetService(typeof(IApiAggrgator<Customer>))`

#### ii. Call Aggregator.GetData() method
You need to call the `GetData()` method with an instance of parameter class derived from `IRequestContext` interface.
- The IRequestContext provides a `Names` property which is a list of string to include all the api names to be included for the given request to fetch aggregated response.
- When `no` names are passed in the paramter then `entire` aggregated response for all configured apis is returned.
- When `subset` of apis are included using names then the returned aggregated response only includes `api responses` from included apis.
- When nested api with `dot` separated api name (eg. `customer.orders.items`) is included then all parent apis also get included for the dependency.
  
##### Example - Control Flow
> Example execution flow for a nested api included in the GetData() parameter.
<img src="https://github.com/CodeShayk/Parsley.Net/blob/master/Images/Parsley.Net.ControlFlow.png" alt="control-flow"/> 

