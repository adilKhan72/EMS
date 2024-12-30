import React, { Component } from "react";
import { ButtonGroup, Row, Container, Col } from "react-bootstrap";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./DatePickerDashboard.css";
import moment from "moment";
import "./AdminDashBoard.css";
import Button from "react-bootstrap/Button";
import { Bar } from "react-chartjs-2";
import "react-dropdown/style.css";
import PopUpModal from "../PopUpModalAdmin/PopUpModal";
import Table from "@material-ui/core/Table";
import TableContainer from "@material-ui/core/TableContainer";
import TablePagination from "@material-ui/core/TablePagination";
import { Link } from "react-router-dom";
import Tooltip from "@material-ui/core/Tooltip";
import InfiniteScroll from "react-infinite-scroll-component";
import Loader from "../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
import Cookies from "js-cookie";
class AdminDashboard extends Component {
  constructor(props) {
    super(props);
    this.state = {
      TimeSpent: {
        TS_DayArray: [],
        TS_HoursArray: [],
        TS_MainTaskNames: [],
      },
      AdminDashboardData: null,
      ProjectId: -1,
      SelectdDate: new Date(),
      ShowSelectedDate: null,
      datePickerIsOpen: false,
      StartDate: null,
      EndDate: null,
      dataFormat: null,
      GraphdataFormat: null,
      ProjectData: null,
      PopUpBit: false,
      Title: "",
      errorMsg: "",
      TimeSpentDataCount: null,
      page: 0,
      rowsPerPage: 10,
      AdminDashboardDataCount: 0,
      GraphShow: "Hide",
      ProjectLazyLoading: {
        ProjectScrollIndex: 1,
        ProjectRecoardPerPage: 20,
        CurrentProjectDataCount: null,
        ShowProjectList: false,
        ProjectHasMore: true,
      },
      DayHourToggle: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      DayHourToggleGraph: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      ShowProjectName: "All Project",
      ManagementCheck: false,
      Loading: true,
    };
  }
  componentDidMount() {
    const GetKey = Cookies.get("encrypted_Type");
    const Getbytes = JSON.parse(Cookies.get("encrypted_Type_Length"));
    const GetEncryptedFormat = { content: GetKey, tag: Getbytes.data };
    const decryptValue = decrypt(GetEncryptedFormat);
    const CookieValue = Cookies.get("Role");
    if (
      decryptValue != CookieValue &&
      (decryptValue != "Staff" ||
        decryptValue != "Admin" ||
        decryptValue != "SuperAdmin")
    ) {
      this.logout();
    }
    var tempDate = moment(new Date()).format("MMMM,YYYY");
    this.setState({ ShowSelectedDate: tempDate });
    //this.LoadAdminTimeSpent();
    //this.LoadProjectName();
    this.LoadProjectNameLazyLoading();
    this.LoadAdminDashobardGrid();
  }
  logout = () => {
    try {
      localStorage.removeItem("token");
      localStorage.removeItem("Video_Upload_Size");
      localStorage.removeItem("S3Obj");
      localStorage.removeItem("userId");
      localStorage.removeItem("EncryptedType");
      localStorage.removeItem("access_token");
      localStorage.removeItem("login_time");
      localStorage.removeItem("expires_in");
      Cookies.remove("Role");
      Cookies.remove("UserID");
      Cookies.remove("UserName");
      Cookies.remove("Design");
      Cookies.remove("encrypted_Type");
      Cookies.remove("encrypted_Type_Length");
      window.location.href = ".";
    } catch (ee) {
      alert(ee);
    }
  };
  LoadAdminDashobardGrid = () => {
    try {
      this.setState({ Loading: true });
      var StartDate = moment(this.state.StartDate).format("YYYY/MM/DD");
      var EndDate = moment(this.state.EndDate).format("YYYY/MM/DD");
      var ProjectIdAsInt = parseInt(this.state.ProjectId, 10);
      var ManagementCheck = this.state.ManagementCheck;
      const AdminDataObj = {
        RequestStartDate: StartDate,
        RequestEndDate: EndDate,
        ProjectID: ProjectIdAsInt,
        ManagementCheck: ManagementCheck,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetDashBoardStats`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AdminDataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (this.state.ProjectData == null) {
          this.setState({
            ProjectData: res.data.Result,
          });
        }
        this.setState({ AdminDashboardData: res.data.Result });
        this.setState({ AdminDashboardDataCount: res.data.Result.length });
        if (this.state.StartDate != null) {
          var tempStartDate = moment(this.state.StartDate).format("MMMM, YYYY");
          var tempEndDate = moment(this.state.EndDate).format("MMMM, YYYY");

          this.setState({
            ShowSelectedDate: tempStartDate + "  to  " + tempEndDate,
          });

          this.setState({ datePickerIsOpen: false });
        } else {
          var tempDate = moment(new Date()).format("MMMM,YYYY");
          this.setState({ ShowSelectedDate: tempDate });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {}
  };
  LoadAdminTimeSpent = () => {
    try {
      var StartDate = moment(this.state.StartDate).format("YYYY/MM/DD");
      var EndDate = moment(this.state.EndDate).format("YYYY/MM/DD");
      var ProjectIdAsInt = parseInt(this.state.ProjectId, 10);
      const UserDataObj = {
        ProjectId: ProjectIdAsInt,
        StartDate: StartDate,
        EndDate: EndDate,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetSpentTaskTime`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserDataObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState(
          {
            TimeSpent: {
              TS_DayArray: res.data.Result.DayArr,
              TS_HoursArray: res.data.Result.HoursArr,
              TS_MainTaskNames: res.data.Result.MainTaskNames,
            },
          },
          () => {
            this.setState({
              TimeSpentDataCount: this.state.TimeSpent.TS_MainTaskNames.length,
            });
          }
        );
        if (this.state.StartDate != null) {
          var tempStartDate = moment(this.state.StartDate).format("MMMM, YYYY");
          var tempEndDate = moment(this.state.EndDate).format("MMMM, YYYY");
          this.setState({
            ShowSelectedDate: tempStartDate + "  to  " + tempEndDate,
          });
        } else {
          var tempDate = moment(new Date()).format("MMMM,YYYY");
          this.setState({ ShowSelectedDate: tempDate });
        }
      });
    } catch (e) {
      //alert(e);
    }
  };
  SelectdDateHandler = (dates) => {
    const [start, end] = dates;
    if (start != null) {
      this.setState({ StartDate: start });
    }
    if (end == null) {
      this.setState({ EndDate: null });
    }

    if (end != null) {
      this.setState({ EndDate: end }, () => {
        var tempStartDate = moment(this.state.StartDate).format("YYYY");
        var tempEndDate = moment(this.state.EndDate).format("YYYY");
        if (tempStartDate - tempEndDate == 0) {
          this.setState(
            {
              ProjectData: null,
            },
            () => {
              this.LoadAdminTimeSpent();
              this.LoadAdminDashobardGrid();
            }
          );
        } else {
          this.setState({ Title: "Error" });
          this.setState({
            errorMsg: "Start Date and End Date must be in same Year!!!",
          });
          this.setState({ StartDate: null });
          this.setState({ EndDate: null });
          this.setState({ datePickerIsOpen: false });
          this.setState({ PopUpBit: true });
        }
      });
    }
  };
  openDatePicker = () => {
    if (this.state.datePickerIsOpen == false) {
      this.setState({
        datePickerIsOpen: true,
      });
    } else {
      this.setState({ StartDate: null });
      this.setState({ EndDate: null });
      this.setState({
        datePickerIsOpen: false,
      });
    }
  };
  /*  LoadProjectName = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: "",
      }).then((res) => {
        this.setState({ ProjectData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  }; */

  LoadProjectNameLazyLoading = () => {
    // try {
    //   const obj = {
    //     page: this.state.ProjectLazyLoading.ProjectScrollIndex,
    //     recsPerPage: this.state.ProjectLazyLoading.ProjectRecoardPerPage,
    //   };
    //   axios({
    //     method: "post",
    //     url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsLazyLoading`,
    //     headers: {
    //       Authorization: "Bearer " + localStorage.getItem("access_token"),
    //     },
    //     data: obj,
    //   }).then((res) => {
    //     this.setState({ ProjectData: res.data.Result });
    //   });
    // } catch (e) {
    //   window.location.href = "/Error";
    //   //alert(e);
    // }
  };

  _onSelect = (e) => {
    this.setState({ GraphShow: "Hide" });
    this.setState({ ShowProjectList: false });
    const index = e.target.selectedIndex;
    const el = e.target.childNodes[index];
    var _projectID = el.getAttribute("ddAttrProjectID");

    if (_projectID != null) {
      this.setState(
        {
          ProjectId: _projectID,
          ShowProjectName: e.target.value,
        },
        () => {
          this.LoadAdminTimeSpent();
          this.LoadAdminDashobardGrid();
        }
      );
    }
  };
  CloseModal = () => {
    this.setState({ PopUpBit: false });
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  render() {
    var ProjectNameList = null;
    ProjectNameList =
      this.state.ProjectData !== null
        ? this.state.ProjectData.map((item) => {
            return (
              <option
                ddAttrProjectID={item.ProjectID}
                ddAttr={item.ProjectDescription}
                value={item.ProjectName}
                //onChange={this._onSelect}
              >
                {item.ProjectName}
              </option>
            );
          })
        : "";

    //bar chart
    const state = {
      labels: this.state.TimeSpent.TS_MainTaskNames,

      datasets:
        this.state.GraphdataFormat == "Days"
          ? [
              {
                label: "Days",
                data: this.state.TimeSpent.TS_DayArray,
                backgroundColor: "#474747",
              },
            ]
          : this.state.GraphdataFormat == "Hours"
          ? [
              {
                label: "Hours",
                data: this.state.TimeSpent.TS_HoursArray,
                borderColor: "rgba(0,0,0,0.09)",
                borderWidth: 0,
                backgroundColor: "#8d9196",
              },
            ]
          : [
              {
                label: "Days",
                data: this.state.TimeSpent.TS_DayArray,
                backgroundColor: "#474747",
              },
            ],
    };
    return (
      <div className="content mt-3">
        <div className="page-heading">
          <div class="container-fluid" id="ems_mobile_dimens">
            <h2 className="m_font_heading ems_dashboard_title">Dashboard</h2>
            <div className="row ems_dashboard_view_inner">
              <div className="col-4"></div>
              <ButtonGroup>
                <div className="col-4 p-0">
                  <div className="project-date-picker">
                    <span className="ems_selected_date">
                      <span className="ems_state mr-2 mt-1">
                        {this.state.ShowSelectedDate}
                      </span>
                      <div
                        className="input-group dashboard-input-group date"
                        style={{ paddingLeft: "0px", paddingRight: "0px" }}
                        id="createdDate"
                      >
                        <div className="CustomAdjustment">
                          <DatePicker
                            className="DashboardDatePicker "
                            popperPlacement="top-end"
                            onChange={this.SelectdDateHandler}
                            startDate={this.state.StartDate}
                            endDate={this.state.EndDate}
                            selectsRange
                            open={this.state.datePickerIsOpen}
                            maxDate={new Date()}
                            dateFormat="MM/yyyy"
                            showYearDropdown
                            showMonthYearPicker
                            onClickOutside={this.openDatePicker}
                          />
                        </div>
                        <button
                          className="input-group-addon"
                          style={{
                            backgroundColor: "#1b1b1b",
                            paddingRight: "15px",
                          }}
                          onClick={this.openDatePicker}
                        >
                          <div>
                            <i className="fa fa-calendar mr-2"></i>
                            <i
                              className="ti-angle-down"
                              style={{ position: "relative", right: "-4px" }}
                            ></i>
                          </div>
                        </button>
                      </div>
                    </span>
                  </div>
                </div>

                <div className="col-4 pr-0">
                  <div
                    className="input-group dashboard-input-group date ems_arr"
                    style={{
                      paddingLeft: "0px",
                      paddingRight: "0px",
                    }}
                    id="createdDate"
                  >
                    <select
                      className="input-group-addon"
                      id="ems_with_management"
                      required
                      autoComplete="off"
                      name="ManagementCheck"
                      onChange={(e) => {
                        var getvalue = e.target.value;
                        if (getvalue == 1) {
                          this.setState(
                            {
                              ManagementCheck: true,
                            },
                            () => {
                              this.LoadAdminTimeSpent();
                              this.LoadAdminDashobardGrid();
                            }
                          );
                        } else {
                          this.setState(
                            {
                              ManagementCheck: false,
                            },
                            () => {
                              this.LoadAdminTimeSpent();
                              this.LoadAdminDashobardGrid();
                            }
                          );
                        }
                      }}
                    >
                      <option value="0">With Management Time</option>
                      <option value="1">Without Management Time</option>
                    </select>
                  </div>
                </div>

                <div className="col-3 pr-0">
                  <div
                    className="input-group dashboard-input-group date"
                    id="ems_select_pro_data"
                  >
                    <select
                      className="input-group-addon"
                      onChange={this._onSelect}
                    >
                      <option
                        className="options_value"
                        ddAttrProjectID="-1"
                        value="All Project"
                      >
                        All Project
                      </option>
                      {ProjectNameList}
                    </select>
                  </div>
                </div>
              </ButtonGroup>
            </div>
          </div>

          {/* <div className="row">
            <div className="col-sm-4">
              <h2 className="m_font_heading">Dashboard</h2>
            </div>
            <div className="col-sm-8 text-right pt-1 ems_mb_grid custom_select_latest_outer">
              
              <div id="ems_dashboard_btn">
                <ButtonGroup>
                  <div className="project-date-picker">
                    <span className="ems_selected_date">
                      {this.state.ShowSelectedDate}
                    </span>
                    <div
                      className="input-group dashboard-input-group date"
                      style={{ paddingLeft: "0px", paddingRight: "0px" }}
                      id="createdDate"
                    >
                      <div className="CustomAdjustment">
                        <DatePicker
                          className="DashboardDatePicker "
                          popperPlacement="top-end"
                          onChange={this.SelectdDateHandler}
                          startDate={this.state.StartDate}
                          endDate={this.state.EndDate}
                          selectsRange
                          open={this.state.datePickerIsOpen}
                          maxDate={new Date()}
                          dateFormat="MM/yyyy"
                          showYearDropdown
                          showMonthYearPicker
                          onClickOutside={this.openDatePicker}
                        />
                      </div>
                      <button
                        className="input-group-addon"
                        style={{
                          backgroundColor: "#1b1b1b",
                          paddingRight: "15px",
                        }}
                        onClick={this.openDatePicker}
                      >
                        <div>
                          <i className="fa fa-calendar mr-2"></i>
                          <i
                            className="ti-angle-down"
                            style={{ position: "relative", right: "-4px" }}
                          ></i>
                        </div>
                      </button>
                    </div>
                  </div>

                  <div className="input-group dashboard-input-group date ems_arr"
                      style={{
                        paddingLeft: "0px",
                        paddingRight: "0px",
                      }}
                      id="createdDate"
                    >
                      <select
                        className="input-group-addon"
                        id="ems_with_management"
                        required
                        autoComplete="off"
                        name="ManagementCheck"
                        onChange={(e) => {
                          var getvalue = e.target.value;
                          if (getvalue == 1) {
                            this.setState(
                              {
                                ManagementCheck: true,
                              },
                              () => {
                                this.LoadAdminTimeSpent();
                                this.LoadAdminDashobardGrid();
                              }
                            );
                          } else {
                            this.setState(
                              {
                                ManagementCheck: false,
                              },
                              () => {
                                this.LoadAdminTimeSpent();
                                this.LoadAdminDashobardGrid();
                              }
                            );
                          }
                        }}
                      >
                        <option value="0">With Management Time</option>
                        <option value="1">Without Management Time</option>
                      </select>
                    </div>

                  <div
                    className="input-group dashboard-input-group date"
                    id="ems_select_pro_data"
                  >
                    <select
                      className="input-group-addon"
                      onChange={this._onSelect}
                    >
                      <option
                      className="options_value"
                        ddAttrProjectID="-1"
                        value="All Project"
                      >
                        All Project
                      </option>
                      {ProjectNameList}
                    </select>
                  </div>
                  
                </ButtonGroup>
              </div>
            </div>
          </div> */}
        </div>
        <div className="row">
          <div className="col-sm-12">
            <div className="card">
              <div className="card-header">
                <div className="row align-items-center">
                  <div className="col-sm-6">
                    <h3 className="m_font_heading">Projects</h3>
                  </div>
                  <div className="col-sm-6 text-right">
                    <h6>
                      <span
                        className={`HoursDays ${this.state.DayHourToggle.dayStyle}`}
                        onClick={() => {
                          this.setState({
                            DayHourToggle: {
                              dayStyle: "btn_underline",
                              hourStyle: "",
                            },
                          });
                          this.setState({ dataFormat: "Days" });
                        }}
                      >
                        Days
                      </span>
                      <span
                        className={`HoursDays ${this.state.DayHourToggle.hourStyle}`}
                        onClick={() => {
                          this.setState({
                            DayHourToggle: {
                              dayStyle: "",
                              hourStyle: "btn_underline",
                            },
                          });
                          this.setState({ dataFormat: "Hours" });
                        }}
                      >
                        Hours
                      </span>
                    </h6>
                  </div>
                </div>
              </div>
              {this.state.Loading ? (
                <Loader />
              ) : (
                <div className="card-body">
                  <div className="table-responsive easing_row">
                    <TableContainer component={"div"}>
                      <Table
                        id="bootstrap-data-table"
                        className="table"
                        //className="table table-striped table-bordered"
                      >
                        <thead>
                          <tr>
                            <th scope="col">Project Name</th>
                            <th scope="col">
                              {this.state.dataFormat == "Days"
                                ? "Completed (Days)"
                                : this.state.dataFormat == "Hours"
                                ? "Completed (Hours)"
                                : "Completed (Days)"}
                            </th>

                            <th scope="col">
                              {this.state.dataFormat == "Days"
                                ? "Budgeted (Days)"
                                : this.state.dataFormat == "Hours"
                                ? "Budgeted (Hours)"
                                : "Budgeted (Days)"}
                            </th>
                            <th scope="col">Progress Bar</th>
                            <th scope="col">Progress Status</th>
                            <th scope="col">Status</th>
                          </tr>
                        </thead>
                        <tbody>
                          {this.state.AdminDashboardData != null
                            ? this.state.AdminDashboardData.slice(
                                this.state.page * this.state.rowsPerPage,
                                this.state.page * this.state.rowsPerPage +
                                  this.state.rowsPerPage
                              ).map((data, index) => {
                                var progressbarStyle = "";
                                data.CompletedPercentage <= 100
                                  ? (progressbarStyle =
                                      "progress-bar bg-gradient")
                                  : (progressbarStyle =
                                      "progress-bar bg-danger");
                                return (
                                  <tr>
                                    <td
                                      className="td_effect"
                                      scope="row"
                                      style={{
                                        paddingRight: 0,
                                        width: "350px",
                                      }}
                                    >
                                      <Link
                                        /* onClick={() => {
                                    this.LoadAdminTimeSpent();
                                  }} */
                                        to={{
                                          pathname: "/ProjectDetails",
                                          ProjectData: {
                                            ProjectID: data.ProjectID,
                                            ProjectName: data.ProjectName,
                                            StartDate: data.StartDate,
                                            EndData: data.EndDate,
                                            ProjectType: data.ProjectType,
                                            ProjectOwner: data.ProjectOwner,
                                            ProjectDescription:
                                              data.ProjectDescription,
                                            Progress: data.CompletedPercentage,
                                            Status: data.isActive,
                                            ClientID: data.ClientID,
                                          },
                                        }}
                                      >
                                        {index + 1}. {data.ProjectName}
                                      </Link>
                                    </td>

                                    <td className="ems_complete_d">
                                      {this.state.dataFormat == "Days"
                                        ? data.TotalDaysSpend
                                        : this.state.dataFormat == "Hours"
                                        ? parseFloat(
                                            data.TotalDaysSpend * 8
                                          ).toFixed(2)
                                        : data.TotalDaysSpend}
                                    </td>

                                    <td className="ems_budgeted">
                                      {this.state.dataFormat == "Days"
                                        ? data.TotalEstimatedDays
                                        : this.state.dataFormat == "Hours"
                                        ? parseFloat(
                                            data.TotalEstimatedDays * 8
                                          ).toFixed(2)
                                        : data.TotalEstimatedDays}
                                    </td>

                                    <td
                                      style={{
                                        width: "300px",
                                        cursor: "pointer",
                                      }}
                                    >
                                      <Tooltip
                                        title="Time Spent on Each Task"
                                        arrow
                                      >
                                        <div
                                          onClick={() => {
                                            this.setState(
                                              {
                                                ProjectId: data.ProjectID,
                                                ShowProjectName:
                                                  data.ProjectName,
                                              },
                                              () => {
                                                this.LoadAdminTimeSpent();
                                                this.LoadAdminDashobardGrid();
                                              }
                                            );
                                            this.setState({
                                              GraphShow: "Show",
                                            });
                                          }}
                                          className="progress"
                                          style={{
                                            position: "relative",
                                            boxShadow: " 3px 4px 10px #888888",
                                          }}
                                        >
                                          <span
                                            style={{
                                              position: "absolute",
                                              zIndex: "1",
                                              left: "50%",
                                              transform: "translatex(-50%)",
                                              color: "black",
                                              top: "-1px",
                                            }}
                                          >
                                            {data.CompletedPercentage}%
                                          </span>
                                          <div
                                            className={progressbarStyle}
                                            role="progressbar"
                                            style={{
                                              width: `${data.CompletedPercentage}%`,
                                            }}
                                            aria-valuenow={`${data.CompletedPercentage}`}
                                            aria-valuemin="0"
                                            aria-valuemax="100"
                                          ></div>
                                        </div>
                                      </Tooltip>
                                    </td>

                                    <td>
                                      {data.IsLimitExceed == false
                                        ? "In-Progress"
                                        : data.CompletedPercentage == 100
                                        ? "Completed"
                                        : "Budget Exceeded"}
                                    </td>
                                    <td>
                                      {data.isActive == 1
                                        ? "Active"
                                        : "In-Active"}
                                    </td>
                                  </tr>
                                );
                              })
                            : null}
                        </tbody>
                      </Table>
                    </TableContainer>
                    <TablePagination
                      component="div"
                      count={this.state.AdminDashboardDataCount}
                      page={this.state.page}
                      onChangePage={this.handleChangePage}
                      rowsPerPage={this.state.rowsPerPage}
                      onChangeRowsPerPage={this.handleChangeRowsPerPage}
                    />
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
        {this.state.GraphShow == "Show" ? (
          <div class="row">
            <div class="col-sm-12">
              <div class="card">
                <div class="card-body">
                  <div class="card-header">
                    <div class="row align-items-center">
                      <div class="col-sm-12">
                        <h3 className="m_font_heading">
                          Time Spent on Each Task
                        </h3>
                        <div
                          className="col-sm-12 text-left"
                          style={{ padding: "15px 0 15px 0" }}
                        >
                          <h6 style={{ display: "inline-block" }}>
                            <span
                              className={`HoursDays ${this.state.DayHourToggleGraph.dayStyle}`}
                              onClick={() => {
                                this.setState({
                                  DayHourToggleGraph: {
                                    dayStyle: "btn_underline",
                                    hourStyle: "",
                                  },
                                });
                                this.setState({ GraphdataFormat: "Days" });
                              }}
                            >
                              Days
                            </span>
                            <span
                              className={`HoursDays ${this.state.DayHourToggleGraph.hourStyle}`}
                              //style={{ marginLeft: "10px" }}
                              onClick={() => {
                                this.setState({
                                  DayHourToggleGraph: {
                                    dayStyle: "",
                                    hourStyle: "btn_underline",
                                  },
                                });
                                this.setState({ GraphdataFormat: "Hours" });
                              }}
                            >
                              Hours
                            </span>
                          </h6>
                          {/*      <button
                            className="btn btn-black"
                            style={{
                              float: "right",
                              marginTop: "-4px",
                              fontSize: "14px",
                              maxHeight: "34px",
                              lineHeight: "18px",
                            }}
                          >
                            All Projects
                          </button> */}
                        </div>
                      </div>
                    </div>
                  </div>
                  <div
                    style={{
                      position: "relative",
                      overflowX: "scroll",
                      maxWidth: "94vw",
                    }}
                  >
                    <div
                      //className="chartWrapper"
                      //class="max-width1200"
                      style={{
                        position: "relative",
                        minHeight: "60vh",
                        width: `${
                          this.state.TimeSpentDataCount > 6
                            ? 10 * this.state.TimeSpentDataCount
                            : 74
                        }vw`,
                      }}
                    >
                      <Bar
                        data={state}
                        options={{
                          /* title:{
                        display:true,
                        text:'Average Rainfall per month',
                        fontSize:20
                        }, */
                          tooltips: {
                            displayColors: false,
                          },
                          responsive: true,
                          maintainAspectRatio: false,

                          scales: {
                            xAxes: [
                              {
                                categoryPercentage: 0.2,
                                barPercentage: 1,
                                ticks: {
                                  callback: function (label, index, labels) {
                                    //wrap graph label on x-axis
                                    if (/\s/.test(label)) {
                                      return label.split(" ");
                                    } else {
                                      return label;
                                    }
                                  },
                                  mirror: true,
                                },
                                gridLines: {
                                  color: "rgba(0, 0, 0, 0)",
                                },
                              },
                            ],
                            yAxes: [
                              {
                                gridLines: {
                                  color: "rgba(0, 0, 0, 0)",
                                },
                                ticks: {
                                  beginAtZero: true,
                                },
                              },
                            ],
                          },
                          legend: {
                            display: false,
                          },
                        }}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ) : null}
        <PopUpModal
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.PopUpBit}
          onHide={this.CloseModal}
        />
      </div>
    );
  }
}
export default AdminDashboard;
