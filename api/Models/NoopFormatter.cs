using System.Runtime.Serialization;

namespace API.Models;

public class NoopFormatter : IFormatter
{
  public object Deserialize(Stream s) => throw new NotImplementedException();
  public void Serialize(Stream s, object o) { }

  public SerializationBinder? Binder { get; set; }
  public StreamingContext Context { get; set; }
#pragma warning disable SYSLIB0050
  public ISurrogateSelector? SurrogateSelector { get; set; }
#pragma warning restore SYSLIB0050
}