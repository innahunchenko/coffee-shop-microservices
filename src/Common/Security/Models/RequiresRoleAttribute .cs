namespace Security.Models
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RequiresRoleAttribute : Attribute
    {
        public Roles[] Roles { get; }

        public RequiresRoleAttribute(params Roles[] roles)
        {
            Roles = roles;
        }
    }
}
