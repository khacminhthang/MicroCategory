using FluentValidation.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace MicroCategory.Infrastructure.Commands
{
    /// <summary>
    /// ICommand 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ICommand<T> : IRequest<T> where T : class
    {
        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>The type of the message.</value>
        [JsonIgnore]
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>The validation result.</value>
        public ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        public abstract bool IsValid();
    }

    /// <summary>
    /// ICommand
    /// </summary>
    public abstract class ICommand : IRequest
    {
        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>The type of the message.</value>
        //[JsonIgnore]
        //public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>The validation result.</value>
        //public ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        //public abstract bool IsValid();
    }
}
