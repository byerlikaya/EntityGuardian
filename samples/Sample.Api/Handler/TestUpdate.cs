using MediatR;
using Sample.Api.ApplicationSpecific;

namespace Sample.Api.Handler
{
    public class TestUpdate : IRequest
    {
        public class TestUpdateHandler : IRequestHandler<TestUpdate>
        {
            private readonly ITestRepository _testRepository;

            public TestUpdateHandler(ITestRepository testRepository)
            {
                _testRepository = testRepository;
            }

            public async Task Handle(TestUpdate request, CancellationToken cancellationToken)
            {
                await _testRepository.UpdatePublisher();
            }
        }
    }
}
