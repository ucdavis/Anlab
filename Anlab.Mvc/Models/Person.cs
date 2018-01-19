namespace AnlabMvc.Models
{
    public class Person
    {
        public string GivenName { get; internal set; }
        public string Surname { get; internal set; }
        public string Kerberos { get; internal set; }
        public string Mail { get; internal set; }
        public string FullName { get; internal set; }
    }

    public class ValidPerson
    {
        public Person Person { get; set; }
        public bool IsInvalid { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
