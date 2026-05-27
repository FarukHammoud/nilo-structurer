using Domain;

namespace Application {
    public class Book : Package {

        private List<INonPathDependentContract> _contracts;
        public Book(List<INonPathDependentContract> contracts) {
            _contracts = contracts;
        }

        public override List<INonPathDependentContract> Contracts => _contracts;
    }
}
