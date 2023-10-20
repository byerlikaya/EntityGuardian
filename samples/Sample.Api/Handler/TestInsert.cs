using MediatR;
using Sample.Api.ApplicationSpecific;

namespace Sample.Api.Handler
{
    public class TestInsert : IRequest
    {
        public class TestHandler : IRequestHandler<TestInsert>
        {
            private readonly ITestRepository _testRepository;

            public TestHandler(ITestRepository testRepository)
            {
                _testRepository = testRepository;
            }

            public async Task Handle(TestInsert request, CancellationToken cancellationToken)
            {
                await _testRepository.SavePublisher();
            }
        }
    }
}
