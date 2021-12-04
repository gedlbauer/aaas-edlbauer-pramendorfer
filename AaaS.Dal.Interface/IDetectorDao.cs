using AaaS.Domain;

namespace AaaS.Dal.Interface
{
    public interface IDetectorDao<TDetector, TAction> : IBaseDao<TDetector> 
        where TDetector : Detector<TAction>
        where TAction : AaaSAction
    {
    }
}
