using System.Reflection;
using Kiss.Validation.Validators;

namespace Kiss.Validation
{
    /// <summary>
    /// Implements a <see cref="NotEmptyValidator"/> for the property.
    /// </summary>
    public class NotEmptyAttribute : ValidatorAttribute
    {

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public override Validator GetValidator(PropertyInfo propertyInfo)
        {
            return new NotEmptyValidator(this.ErrorMessage, propertyInfo);
        }

    }
}