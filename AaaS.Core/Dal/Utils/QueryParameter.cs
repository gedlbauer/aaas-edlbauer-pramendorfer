namespace AaaS.Core.Dal.Utils
{
  public class QueryParameter
  {
    public QueryParameter(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; }
    public object Value { get; }
  }
}
