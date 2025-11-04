using System;
using System.Collections.Generic;
using System.Linq;

namespace parsley
{
    /// <summary>
    /// Represents the result of a parsing operation with explicit success/failure semantics
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure 
        { 
            get { return !IsSuccess; } 
        }
        public T Value { get; }
        public IList<string> Errors { get; }

        private Result(bool isSuccess, T value, IList<string> errors)
        {
            if (isSuccess && errors != null && errors.Count > 0)
                throw new ArgumentException("Success results should not have errors", nameof(errors));

            if (!isSuccess && value != null)
                throw new ArgumentException("Failure results should not have values", nameof(value));

            IsSuccess = isSuccess;
            Value = value;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default(T), new List<string> { error });
        }

        public static Result<T> Failure(IList<string> errors)
        {
            return new Result<T>(false, default(T), errors);
        }

        public static Result<T> Failure(string error, IList<string> errors)
        {
            var allErrors = new List<string>();
            if (!string.IsNullOrEmpty(error))
                allErrors.Add(error);
            
            if (errors != null)
                allErrors.AddRange(errors);
            
            return new Result<T>(false, default(T), allErrors);
        }
    }
}