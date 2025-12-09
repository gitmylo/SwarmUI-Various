using SwarmUI.Accounts;
using SwarmUI.Builtin_ComfyUIBackend;

namespace GitMylo.SwarmVarious;

public abstract class ComfyNodeFeature
{
    public abstract void Register();

    /// <summary>
    /// Util used to specify values for option inputs.
    /// </summary>
    /// <param name="values"></param>
    public Func<Session,List<string>> Values(params string[] values)
    {
        return (x => values.ToList());
    }
}