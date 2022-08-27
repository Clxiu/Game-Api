using GameApi.Model;
using MediatR;

namespace GameApi.MovesQueries
{

    public class GetMoveQuery : IRequest<MoveType>
    {

    }

    public class GetMoveHandler : IRequestHandler<GetMoveQuery, MoveType>
    {
        public Task<MoveType> Handle(GetMoveQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}