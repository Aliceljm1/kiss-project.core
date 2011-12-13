using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="LengthValidator"/> for the property.
    /// </summary>
    public class LengthAttribute : ValidatorAttribute
    {

        #region Private Fields

        private uint maxLength;
        private uint minLength;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        public uint MaxLength
        {
            get { return this.maxLength; }
        }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        /// <value>The minimum length</value>
        public uint MinLength
        {
            get { return this.minLength; }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthAttribute"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum length.</param>
        public LengthAttribute(uint maxLength)
            : this(uint.MinValue, maxLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthAttribute"/> class.
        /// </summary>
        /// <param name="minLength">The minimum length..</param>
        /// <param name="maxLength">The maximum length.</param>
        public LengthAttribute(uint minLength, uint maxLength)
        {
            this.maxLength = maxLength;
            this.minLength = minLength;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new LengthValidator(this.ErrorMessage, propertyInfo, this.minLength, this.maxLength);
        }

        #endregion

    }
}