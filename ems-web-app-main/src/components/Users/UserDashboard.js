import React, { Component } from "react";
import axios from "axios";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./DatePickerDashboard.css";
import moment from "moment";
import Cookies from "js-cookie";
import "./UserDashBoard.css";
import Table from "@material-ui/core/Table";
import TableContainer from "@material-ui/core/TableContainer";
import TablePagination from "@material-ui/core/TablePagination";
import Loader from "../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
class UserDashboard extends Component {
  constructor(props) {
    super(props);
    this.state = {
      UserState: null,
      iStaffID: Cookies.get("UserID"),
      SelectdDate: new Date(),
      ShowSelectedDate: null,
      datePickerIsOpen: false,
      StartDate: null,
      EndDate: null,
      dataFormat: null,
      DayHourToggle: {
        dayStyle: "btn_underline",
        hourStyle: "",
      },
      page: 0,
      rowsPerPage: 10,
      UserStateDataCount: 0,
      loading: true,
    };
  }
  componentDidMount() {
    if (Cookies.get("UserID") == null && Cookies.get("UserID") == undefined) {
      window.location.href = ".";
    }
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
    this.LoadUserState();
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
      //window.location.href = ".";
    } catch (ee) {
      alert(ee);
    }
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  LoadUserState = () => {
    try {
      this.setState({ Loading: true });
      const UserDataObj = {
        Month: `${moment(this.state.SelectdDate).format("MM")}`,
        Year: `${moment(this.state.SelectdDate).format("YYYY")}`,
        iStaffID: this.state.iStaffID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetDashBoardStatsForStaff`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserDataObj,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ UserState: res.data.Result });
        this.setState({ UserStateDataCount: res.data.Result.length });
        this.setState({ Loading: false });
      });
    } catch (e) {
      //alert(e);
    }
  };
  LoadUserStateSearch = () => {
    try {
      var StartDate = moment(this.state.StartDate).format("YYYY/MM/DD");
      var EndDate = moment(this.state.EndDate).format("YYYY/MM/DD");

      const UserDataObj = {
        StartDate: StartDate,
        EndDate: EndDate,
        iStaffID: this.state.iStaffID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Dashboard/GetDashBoardStatsForStaffSearch`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserDataObj,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ UserState: res.data.Result });
        var tempStartDate = moment(this.state.StartDate).format(
          "DD, MMMM, YYYY"
        );
        var tempEndDate = moment(this.state.EndDate).format("DD, MMMM, YYYY");

        this.setState({
          ShowSelectedDate: tempStartDate + "  to  " + tempEndDate,
        });
        this.setState({ StartDate: null });
        this.setState({ EndDate: null });
        this.setState({ datePickerIsOpen: false });
      });
    } catch (e) {
      //alert(e);
    }
  };

  SelectdDateHandler = (dates) => {
    const [start, end] = dates;
    this.setState({ StartDate: start });

    if (end != null) {
      this.setState({ EndDate: end }, () => {
        this.LoadUserStateSearch();
      });
    }
  };
  openDatePicker = () => {
    if (this.state.datePickerIsOpen == false) {
      this.setState({
        datePickerIsOpen: true,
      });
    } else {
      this.setState({
        datePickerIsOpen: false,
      });
    }
  };

  render() {
    return (
      <div className="content mt-3">
        <div className="page-heading">
          <div className="row">
            <div className="col-sm-4">
              <h2 className="m_font_heading">Dashboard</h2>
            </div>

            <div className="col-sm-8 text-right">
              <div className="project-date-picker">
                <span className="ems_selected_date">
                  <div
                    className="input-group dashboard-input-group date"
                    style={{ paddingLeft: "0px", paddingRight: "0px" }}
                    id="createdDate"
                  >
                    <span className="ems_state mr-2 mt-1">
                      {this.state.ShowSelectedDate}
                    </span>
                    <div className="CustomAdjustment">
                      <DatePicker
                        //shouldCloseOnSelect={true}
                        className="DashboardDatePicker "
                        popperPlacement="top-end"
                        onChange={this.SelectdDateHandler}
                        startDate={this.state.StartDate}
                        endDate={this.state.EndDate}
                        selectsRange
                        open={this.state.datePickerIsOpen}
                        showYearDropdown
                        showMonthDropdown
                        maxDate={new Date()}
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
          </div>
        </div>
        <div className="row">
          <div className="col-sm-12">
            <div className="card">
              <div className="card-header">
                <div className="row align-items-center">
                  <div className="col-sm-6">
                    <h3>Projects</h3>
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
                        style={{ marginLeft: "10px" }}
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
                    {/*  <button type="button" className="btn-black">
                      View All
                    </button> */}
                  </div>
                </div>
              </div>
              {this.state.Loading ? (
                <Loader />
              ) : (
                <div className="card-body">
                  <div className="table-responsive">
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
                                ? "Contributed (Days)"
                                : this.state.dataFormat == "Hours"
                                ? "Contributed (Hours)"
                                : "Contributed (Days)"}
                            </th>

                            <th scope="col">
                              {this.state.dataFormat == "Days"
                                ? "Total Spent (Days)"
                                : this.state.dataFormat == "Hours"
                                ? "Total Spent (Hours)"
                                : "Total Spent (Days)"}
                            </th>
                            <th scope="col" width="250">
                              Contribution (%)
                            </th>
                            {/*  <th scope="col">Status</th> */}
                          </tr>
                        </thead>
                        <tbody>
                          {this.state.UserState != null
                            ? this.state.UserState.slice(
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
                                    <td scope="row">
                                      {index + 1}. {data.ProjectName}
                                    </td>
                                    <td className="ems_td_data_align">
                                      {this.state.dataFormat == "Days"
                                        ? data.TotalDaysSpend
                                        : this.state.dataFormat == "Hours"
                                        ? parseFloat(
                                            data.TotalDaysSpend * 8
                                          ).toFixed(2)
                                        : data.TotalDaysSpend}
                                    </td>
                                    <td className="ems_total_days">
                                      {this.state.dataFormat == "Days"
                                        ? data.TotalEstimatedDays
                                        : this.state.dataFormat == "Hours"
                                        ? parseFloat(
                                            data.TotalEstimatedDays * 8
                                          ).toFixed(2)
                                        : data.TotalEstimatedDays}
                                    </td>
                                    <td>
                                      <div
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
                                    </td>
                                    {/*  <td>Budget Exeeded</td> */}
                                  </tr>
                                );
                              })
                            : null}
                        </tbody>
                      </Table>
                    </TableContainer>
                    <TablePagination
                      component="div"
                      count={this.state.UserStateDataCount}
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
        {/*   {this.state.EndDate != null ? this.LoadUserStateSearch() : null} */}
      </div>
    );
  }
}
export default UserDashboard;
