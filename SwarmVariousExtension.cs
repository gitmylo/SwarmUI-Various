using GitMylo.SwarmVarious.features;
using SwarmUI.Core;
using SwarmUI.Text2Image;
using SwarmUI.Utils;

namespace GitMylo.SwarmVarious;

public class SwarmVariousExtension : Extension
{
    public static T2IParamGroup SwarmVariousGroup;
    
    public override void OnInit()
    {
        SwarmVariousGroup = new T2IParamGroup("[Extension] Swarm various", Description: "Options from the swarm various extension", OrderPriority:0);
        
        // Init features
        Register(typeof(ScaleROPE));
    }

    /// <summary>
    /// Allows for `Register(typeof(feature));` as `new feature().Register();`
    /// </summary>
    void Register(Type type)
    {
        (Activator.CreateInstance(type) as ComfyNodeFeature)?.Register();
    }
}