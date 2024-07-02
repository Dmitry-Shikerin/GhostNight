using System;
using Sources.BoundedContexts.Ids.Domain.Constant;
using Sources.BoundedContexts.Tutorials.Domain.Data;
using Sources.Frameworks.GameServices.Repositories.Domain.Interfaces;

namespace Sources.BoundedContexts.Tutorials.Domain.Models
{
    public class Tutorial : IEntity
    {
        public Tutorial(TutorialDto tutorialDto)
            : this(tutorialDto.Id, tutorialDto.HasCompleted)
        {
        }

        public Tutorial()
            : this(ModelId.Tutorial, false)
        {
        }

        private Tutorial(string id, bool hasCompleted)
        {
            Id = id;
            HasCompleted = hasCompleted;
        }

        public bool HasCompleted { get; set; }

        public string Id { get; }

        public Type Type => GetType();
    }
}