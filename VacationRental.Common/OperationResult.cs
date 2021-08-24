using System;
using System.Collections.Generic;
using System.Linq;

namespace VacationRental.Common
{
    public class OperationResult
    {
        public bool Success { get; protected set; }

        public List<string> Errors { get; protected set; }

        public Exception Exception { get; protected set; }

        internal OperationResult()
        {
            if (Errors == null) Errors = new List<string>();
            Success = true;
        }

        internal OperationResult(string message) : this()
        {
            Success = false;
            Errors.Add(message);
        }

        internal OperationResult(Exception ex) : this()
        {
            Success = false;
            Exception = ex;
        }

        public bool IsException()
        {
            return Exception != null;
        }

        public bool HasErrors()
        {
            return Errors != null && Errors.Any();
        }

        public void AddException(Exception ex)
        {
            Success = false;
            Exception = ex;
        }

        public void AddError(string error)
        {
            if (Errors == null) Errors = new List<string>();
            Success = false;
            Errors.Add(error);
        }

        public void AddErrors(IList<string> errors)
        {
            if (Errors == null) Errors = new List<string>();
            Success = false;
            Errors.AddRange(errors);
        }

        public static implicit operator string(OperationResult result) => result.Errors.ToString();

        public static implicit operator bool(OperationResult result) => result.Success;
    }

    public class OperationResult<T> : OperationResult
    {
        public T Value { get; set; }

        internal OperationResult() : base() { }

        internal OperationResult(T value) : base()
        {
            Value = value;
        }

        internal OperationResult(T value, string errorMessage) : base(errorMessage)
        {
            Value = value;
        }

        internal OperationResult(T value, Exception ex) : base(ex)
        {
            Value = value;
        }
    }

    public static class OperationResultHelpers
    {
        public static OperationResult Ok()
        {
            return new OperationResult();
        }

        public static OperationResult Error(string errorMessage)
        {
            return new OperationResult(errorMessage);
        }

        public static OperationResult ExceptionResult(Exception ex)
        {
            return new OperationResult(ex);
        }

        public static OperationResult<U> Ok<U>()
        {
            return new OperationResult<U>();
        }

        public static OperationResult<U> Ok<U>(U value)
        {
            return new OperationResult<U>(value);
        }

        public static OperationResult<U> Error<U>(string errorMessage, U value)
        {
            return new OperationResult<U>(value, errorMessage);
        }
    }
}