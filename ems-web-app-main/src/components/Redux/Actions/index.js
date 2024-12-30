export const Refresh = "Refresh";
export const NotRefresh = "NotRefresh";
export const RefreshReport = "RefreshReport";
export const NotRefreshReport = "NotRefreshReport";

export const RefreshAction = () => {
  return {
    type: "Refresh",
  };
};

export const NotRefreshAction = () => {
  return {
    type: "NotRefresh",
  };
};

export const RefreshReportAction = () => {
  return {
    type: "RefreshReport",
  };
};

export const NotRefreshReportAction = () => {
  return {
    type: "NotRefreshReport",
  };
};
