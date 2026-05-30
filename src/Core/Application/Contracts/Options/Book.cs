using Domain;

namespace Application {
    public class Book : Package {

        private List<IPathIndependentContract> _contracts;
        public Book(List<IPathIndependentContract> contracts) {
            _contracts = contracts;
        }

        public override List<IPathIndependentContract> Contracts => _contracts;
    }
}
