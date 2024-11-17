import { Counter } from "./components/Counter";
import { PricerTab } from "./components/PricerTab";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/pricer-tab',
    element: <PricerTab />
  }
];

export default AppRoutes;
