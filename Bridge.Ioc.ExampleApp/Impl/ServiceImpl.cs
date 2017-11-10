using Bridge.Ioc.ExampleApp.Abstract;

namespace Bridge.Ioc.ExampleApp.Impl
{
    public class ServiceImpl : IService
    {
        private ISubService _subService;
        public ServiceImpl(ISubService subService)
        {
            _subService = subService;
        }

        public void RunService()
        {
            _subService.Method();
        }

        public string TellMyType()
        {
            return GetType().ToString();
        }
    }
}
