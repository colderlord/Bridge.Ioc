using Bridge.Ioc.ExampleApp.Abstract;
using System;

namespace Bridge.Ioc.ExampleApp.Impl
{
    public class SubServiceImpl : ISubService
    {
        private ILazy<IService> _lazyService;
        private IService service { get { return _lazyService.Value(); } }
        
        // If we put only IService we will receive "RecurciveException"
        public SubServiceImpl(ILazy<IService> lazyService)
        {
            _lazyService = lazyService;
        }

        public void Method()
        {
            Console.WriteLine(service.TellMyType());
        }
    }
}
