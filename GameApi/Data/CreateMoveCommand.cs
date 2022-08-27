using GameApi.Model;
using MediatR;

namespace GameApi.MovesCommands
{
    public class CreateMoveCommand : IRequest
    {
        public GameStep GameStep { get; set; } = default!;

        public CreateMoveCommand(GameStep gameStep)
        {
            GameStep = gameStep ?? throw new ArgumentNullException(nameof(gameStep));
        }
    }

    public class CreateMoveHandler : AsyncRequestHandler<CreateMoveCommand>
    {
        protected override Task Handle(CreateMoveCommand request, CancellationToken cancellationToken)
        {
            // request.GameStep
            throw new NotImplementedException();
        }
    }
}