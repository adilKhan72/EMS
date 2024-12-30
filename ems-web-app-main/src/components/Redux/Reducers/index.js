import refreshbitReducer from "./RefreshBit";
import refreshReportReducer from "./RefreshReportData";
import { combineReducers } from "redux";

const rootReducers = combineReducers({
  RefreshBit: refreshbitReducer,
  RefreshReportDataBit: refreshReportReducer,
});
export default rootReducers;
