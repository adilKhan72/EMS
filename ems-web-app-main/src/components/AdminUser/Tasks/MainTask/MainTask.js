import React, { Component } from "react";
import axios from "axios";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import AddMainTaskPopup from "./AddMainTaskPopup";
import FilterMainTaskPopup from "./FilterMainTaskPopup";
import "../../../Material_UI_Inputs/Material_UI_Inputs.css";
import Cookies from "js-cookie";
import SureMsgPopUp from "./SureMsgPopUp";
import Loader from "../../../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
class MainTask extends Component {
  constructor(props) {
    super(props);
    this.state = {
      loading: true,
      AddMainTaskShow: false,
      AddMainTaskTitle: null,
      AddMainTaskButton: null,
      FilterMainTaskPopup: false,
      lstMainTask: [],
      page: 0,
      rowsPerPage: 10,
      MainTaskHandle: {
        MainTaskName: "",
        Status: "",
        Id: 0,
      },
      FilterMainTask: {
        MainTaskName: "",
        Status: "",
        Id: 0,
      },
      error_class: {
        MainTaskName: "",
        Status: "",
      },
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
    this.LoadMainTasks();
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
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  LoadMainTasks = () => {
    try {
      this.setState({
        FilterMainTask: {
          MainTaskName: "",
        },
      });
      this.setState({ Loading: true });
      this.setState({ FilterMainTaskPopup: false });
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMainTask: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddMainTaskHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        MainTaskHandle: {
          ...this.state.MainTaskHandle,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  AddMainTasks = () => {
    try {
      this.setState({ Loading: true });
      var AddMainTask = {
        Id: 0,
        MainTaskName: null,
        Active: 0,
        Case: null,
      };

      if (this.state.MainTaskHandle.Id == 0) {
        AddMainTask = {
          Id: 0,
          MainTaskName: this.state.MainTaskHandle.MainTaskName,
          Active: this.state.MainTaskHandle.Status == "Active" ? 1 : 0,
          Case: "add",
        };
      } else {
        AddMainTask = {
          Id: this.state.MainTaskHandle.Id,
          MainTaskName: this.state.MainTaskHandle.MainTaskName,
          Active: this.state.MainTaskHandle.Status == "Active" ? 1 : 0,
          Case: null,
        };
      }

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/InsertMainTask`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddMainTask,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({
            MainTaskHandle: {
              MainTaskName: "",
              Status: "",
              Id: 0,
            },
          });
          this.setState({ AddMainTaskShow: false }, () => {
            if (this.state.FilterMainTask.MainTaskName !== "") {
              this.SearchMainTask();
            } else {
              this.LoadMainTasks();
            }
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SearchMainTaskfiltervalidation = () => {
    if (this.state.FilterMainTask.MainTaskName !== "") {
      this.SearchMainTask();
    } else {
      this.setState({ Title: "Filter Alert" });
      this.setState({
        errorMsg: "Please Select  Filter!",
      });
      this.setState({ ShowMsgPopUp: true });
    }
  };
  SearchMainTask = () => {
    try {
      this.setState({ Loading: true });
      const SearchMainTask = {
        MainTaskName: this.state.FilterMainTask,
        IsFilter: 0, //Used to Get Active Maintask
      };
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetFilterMainTaskList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: SearchMainTask,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ FilterMainTaskPopup: false });
        this.setState({ lstMainTask: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  FilterMainTaskHandleChange = (e) => {
    this.setState({ FilterMainTask: e.target.value });
  };
  validatePopUp = () => {
    var checkError = false;
    if (this.state.MainTaskHandle.MainTaskName === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          MainTaskName: "input_error",
        },
      }));
    }
    if (this.state.MainTaskHandle.MainTaskName !== "") {
      var checkTask = this.state.lstMainTask.filter(
        (Name) =>
          Name.MainTaskName.trim().toLowerCase() ==
          this.state.MainTaskHandle?.MainTaskName.trim().toLowerCase()
      );
      if (
        checkTask.length > 0 &&
        checkTask[0]?.Id !== this.state.MainTaskHandle?.Id
      ) {
        checkError = true;
        this.setState({ Title: "MainTask Alert" });
        this.setState({
          errorMsg:
            `MainTask "` +
            this.state.MainTaskHandle?.MainTaskName +
            `" Name Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      }
      // else {
      //   if (checkTask.length > 1) {
      //     checkError = true;
      //     this.setState({ Title: "MainTask Alert" });
      //     this.setState({
      //       errorMsg:
      //         `MainTask "` +
      //         this.state.MainTaskHandle?.MainTaskName +
      //         `" Name Already exit`,
      //     });
      //     this.setState({ ShowMsgPopUp: true });
      //   }
      // }
    }

    /* if (this.state.MainTaskHandle.Status == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          Status: "input_error",
        },
      }));
    } */
    if (!checkError) {
      this.AddMainTasks();
    }
  };
  CheckPopUp = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CloseModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  HandelErrorRemove = (name) => {
    if (name == "MainTaskName") {
      if (this.state.MainTaskHandle.MainTaskName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            MainTaskName: "",
          },
        }));
      }
    }
    if (name == "Status") {
      if (this.state.MainTaskHandle.Status == "") {
        //checkError = true;
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Status: "",
          },
        }));
      }
    }
  };
  render() {
    return (
      <div>
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="row rpt_mbl_header">
                <div
                  className="col-sm-4 text-left"
                  style={{ marginBottom: "10px" }}
                >
                  <h3 className="m_font_heading">Main Tasks</h3>
                </div>

                <div
                  className="col-sm-8 text-right"
                  id="ems_user_manage_projects"
                >
                  <button
                    onClick={() => {
                      this.LoadMainTasks();
                    }}
                    type="button"
                    className="btn-black"
                    data-toggle="modal"
                    data-target="#FilterSubTask"
                    style={{
                      float: "right",
                      marginLeft: "10px",
                      marginBottom: "10px",
                    }}
                  >
                    Clear Search
                  </button>
                  <button
                    onClick={() => {
                      this.setState({ FilterMainTaskPopup: true });
                    }}
                    type="button"
                    className="btn-black"
                    data-toggle="modal"
                    data-target="#FilterSubTask"
                    style={{
                      float: "right",
                      marginLeft: "10px",
                      marginBottom: "10px",
                    }}
                  >
                    Task Filter <i className="menu-icon fa fa-plus"></i>
                  </button>

                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={() => {
                        this.setState({
                          MainTaskHandle: {
                            MainTaskName: "",
                            Status: "",
                            Id: 0,
                          },
                          AddMainTaskShow: true,
                          AddMainTaskTitle: "Add MainTask",
                          AddMainTaskButton: "Add MainTask",
                        });
                      }}
                      type="button"
                      className="btn-black mr-1"
                      data-toggle="modal"
                      data-target="#AddSubTask"
                      style={{
                        float: "right",
                        paddingLeft: "30px",
                        paddingRight: "30px",
                      }}
                    >
                      Add Main Task
                    </button>
                  ) : (
                    ""
                  )}
                </div>
              </div>
            </div>
          </div>
          {this.state.Loading ? (
            <Loader />
          ) : (
            <div className="card custom_v_ems" style={{ margintop: "20px" }}>
              <div className="card-body" style={{ paddingTop: "0px" }}>
                <TableContainer className="table-responsive" component={"div"}>
                  <Table className="Tabel">
                    <TableHead className="main_tbl_head">
                      <TableRow>
                        <TableCell>Task Name</TableCell>
                        <TableCell align="center">Active </TableCell>
                        {/* <TableCell align="center">Estimated Duration</TableCell>
                      <TableCell align="center">Task Type</TableCell>
                      <TableCell align="center">Phase</TableCell> */}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {this.state.lstMainTask.length > 0 ? (
                        this.state.lstMainTask
                          .slice(
                            this.state.page * this.state.rowsPerPage,
                            this.state.page * this.state.rowsPerPage +
                              this.state.rowsPerPage
                          )
                          .map((item, key) => {
                            return (
                              <TableRow
                                style={
                                  Cookies.get("Role") === "SuperAdmin"
                                    ? { cursor: "pointer" }
                                    : { cursor: "" }
                                }
                                onClick={() => {
                                  if (Cookies.get("Role") == "SuperAdmin") {
                                    this.setState({
                                      MainTaskHandle: {
                                        MainTaskName: item.MainTaskName,
                                        Status:
                                          item.Active == true
                                            ? "Active"
                                            : "In-Active",
                                        Id: item.Id,
                                      },
                                      AddMainTaskShow: true,
                                      AddMainTaskTitle: "Update MainTask",
                                      AddMainTaskButton: "Update MainTask",
                                    });
                                  }
                                }}
                              >
                                <TableCell component="th" scope="row">
                                  {item.MainTaskName}
                                </TableCell>
                                <TableCell align="center">
                                  {String(item.Active)}
                                </TableCell>
                              </TableRow>
                            );
                          })
                      ) : (
                        <TableRow>
                          <TableCell
                            component="th"
                            scope="row"
                            style={{ textAlign: "center" }}
                            colSpan={8}
                          >
                            No Record Avaliable
                          </TableCell>
                        </TableRow>
                      )}
                    </TableBody>
                  </Table>
                </TableContainer>
                <TablePagination
                  component="div"
                  count={this.state.lstMainTask.length}
                  page={this.state.page}
                  onChangePage={this.handleChangePage}
                  rowsPerPage={this.state.rowsPerPage}
                  onChangeRowsPerPage={this.handleChangeRowsPerPage}
                />
              </div>
            </div>
          )}
        </div>
        <AddMainTaskPopup
          AddMainTaskTitle={this.state.AddMainTaskTitle}
          AddMaintaskButton={this.state.AddMainTaskButton}
          AddMainTaskShow={this.state.AddMainTaskShow}
          AddMainTaskHandle={this.state.MainTaskHandle}
          AddMainTaskHandleChange={this.AddMainTaskHandleChange}
          // AddMainTaskFun={this.AddMainTasks}
          closeAddSubTask={() => {
            this.setState({ AddMainTaskShow: false });
          }}
          errorclass={this.state.error_class}
          AddMainTaskFun={this.validatePopUp}
          HandelErrorRemove={this.HandelErrorRemove}
        />
        <FilterMainTaskPopup
          FilterMainTask={this.state.FilterMainTask}
          FilterMainTaskHandleChange={this.FilterMainTaskHandleChange}
          FilterMainTaskPopup={this.state.FilterMainTaskPopup}
          // ClearSearch={this.LoadMainTasks}
          FilterMainTaskFun={this.SearchMainTaskfiltervalidation}
          closeFilterSubTask={() => {
            this.setState({ FilterMainTaskPopup: false });
          }}
        />
        <SureMsgPopUp
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ShowMsgPopUp}
          onConfrim={this.CheckPopUp}
          onHide={this.CloseModal}
          DeleteStatus={this.ChangeStatus}
        />
      </div>
    );
  }
}
export default MainTask;
