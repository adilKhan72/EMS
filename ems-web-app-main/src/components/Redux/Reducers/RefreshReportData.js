import { RefreshReport, NotRefreshReport } from "../Actions/index";

const refreshReportReducer = (state = false, action) => {
  switch (action.type) {
    case RefreshReport:
      return (state = true);
      break;
    case NotRefreshReport:
      return (state = false);
      break;
    default:
      return (state = false);
  }
};
export default refreshReportReducer;
