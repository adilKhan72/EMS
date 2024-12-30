import { Refresh, NotRefresh } from "../Actions/index";

const refreshbitReducer = (state = false, action) => {
  switch (action.type) {
    case Refresh:
      return (state = true);
      break;
    case NotRefresh:
      return (state = false);
      break;
    default:
      return (state = false);
  }
};
export default refreshbitReducer;
