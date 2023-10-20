using MediatR;
using Sample.Api.ApplicationSpecific;

namespace Sample.Api.Handler
{
    public class TestDelete : IRequest
    {
        public class TestDeleteHandler : IRequestHandler<TestDelete>
        {
            private readonly ITestRepository _testRepository;

            public TestDeleteHandler(ITestRepository testRepository)
            {
                _testRepository = testRepository;
            }

            public async Task Handle(TestDelete request, CancellationToken cancellationToken)
            {
                await _testRepository.DeletePublisher();
            }
        }
    }
}
