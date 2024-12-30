import React, { Component } from "react";
import axios from "axios";
import moment from "moment";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import AddSubTaskPopup from "./AddSubTaskPopup";
import FilterSubTaskPopup from "./FilterSubTaskPopup";
import Cookies from "js-cookie";
import SureMsgPopUp from "../MainTask/SureMsgPopUp";
import Loader from "../../../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
class SubTask extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Loading: true,
      AddSubTaskShow: false,
      FilterSubTaskPopup: false,
      lstSubTask: [],
      page: 0,
      rowsPerPage: 10,
      FilterSubTask: {
        Id: 0,
        SubTaskName: "",
        ProjectName: "",
        MainTaskID: null,
        MainTaskName: "",
        ProjectID: 0,
      },
      SubTaskHandle: {
        Id: 0,
        SubTaskName: "",
        ProjectName: "",
        MainTaskID: null,
        MainTaskName: "",
        EstimatedDuration: 0,
        Comments: null,
      },
      AddSubTaskTitle: null,
      AddSubtaskButton: null,
      lstProjectName: null,
      lstMainTask: null,
      error_class: {
        ProjectName: "",
        MainTaskName: "",
        SubTaskName: "",
      },
      ProjectIDDrodown: 0,
      ProjectNameDropdown: null,
      MainTaskNameDropdown: null,
      MainTaskIDDropdown: 0,
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
    this.LoadSubTasks();
    this.LoadProjectName();
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
  LoadSubTasks = () => {
    this.setState({ Loading: true });
    try {
      this.setState({
        FilterSubTask: {
          SubTaskName: "",
        },
        ProjectIDDrodown: 0,
        ProjectNameDropdown: null,
        MainTaskIDDropdown: 0,
        MainTaskNameDropdown: null,
      });
      this.setState({ FilterSubTaskPopup: false });

      const LoadSubTasks = {
        TaskName: null,
        ProjectID: -1,
      };
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Tasks/GetTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: LoadSubTasks,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstSubTask: res.data.Result }, () => {
          this.setState({ Loading: false });
        });
        // this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadProjectName = () => {
    this.setState({ Loading: true });
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: "",
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstProjectName: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadMainTasks = () => {
    this.setState({ Loading: true });
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMainTask: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SearchSubTaskfiltervalidation = () => {
    this.SearchSubTask();
    // if (
    //   this.state.FilterSubTask.SubTaskName !== "" ||
    //   this.state.ProjectIDDrodown > 0 ||
    //   this.state.MainTaskIDDropdown > 0
    // ) {
    //   this.SearchSubTask();
    // } else {
    //   this.setState({ Title: "Filter Alert" });
    //   this.setState({
    //     errorMsg: "Please Select  Filter!",
    //   });
    //   this.setState({ ShowMsgPopUp: true });
    // }
  };
  SearchSubTask = () => {
    this.setState({ Loading: true, page: 0 });
    try {
      const SearchSubTask = {
        TaskName: this.state.FilterSubTask?.TaskName,
        ProjectID:
          this.state.ProjectIDDrodown == 0
            ? -1
            : parseInt(this.state.ProjectIDDrodown, 10),
        MainTaskID:
          this.state.MainTaskIDDropdown == 0
            ? -1
            : parseInt(this.state.MainTaskIDDropdown, 10),
      };
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Tasks/GetTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: SearchSubTask,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstSubTask: res.data.Result }, () => {
          this.setState({ FilterSubTaskPopup: false });
        });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddSubTask = () => {
    try {
      var AddSubTask = {
        Id: 0,
        TaskName: 0,
        MainTaskID: 0,
        ProjectName: null,
        TaskTypeName: "None",
        EstimatedDuration: 0,
        Phase: 0,
      };

      if (this.state.SubTaskHandle.Id == 0) {
        AddSubTask = {
          Id: 0,
          TaskName: this.state.SubTaskHandle.SubTaskName,
          MainTaskID: this.state.SubTaskHandle.MainTaskID,
          ProjectName: this.state.SubTaskHandle.ProjectName,
          TaskTypeName: "None",
          EstimatedDuration: this.state.SubTaskHandle.EstimatedDuration,
          Phase: 0,
          Comments: this.state.SubTaskHandle.Comments,
        };
      } else {
        AddSubTask = {
          Id: this.state.SubTaskHandle.Id,
          TaskName: this.state.SubTaskHandle.SubTaskName,
          MainTaskID: this.state.SubTaskHandle.MainTaskID,
          ProjectName: this.state.SubTaskHandle.ProjectName,
          TaskTypeName: "None",
          EstimatedDuration: this.state.SubTaskHandle.EstimatedDuration,
          Phase: 0,
          Comments: this.state.SubTaskHandle.Comments,
        };
      }

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Tasks/AddTask`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddSubTask,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState(
            {
              SubTaskHandle: {
                Id: 0,
                ProjectName: "",
                SubTaskName: "",
                MainTask: "",
              },
              AddSubTaskShow: false,
            },
            () => {
              this.SearchSubTask();
            }
          );
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  DeleteSubTask = () => {
    try {
      this.setState({ Loading: true });
      const DeleteTask = {
        ID: this.state.SubTaskHandle.Id,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Tasks/DeleteTask`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DeleteTask,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ AddSubTaskShow: false });
        this.LoadSubTasks();
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadMainTasksMappedByFilter = (ProjectID) => {
    try {
      const ProjectId = {
        ProjectID: ProjectID,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTaskMappedList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectId,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstMainTask: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddSubTaskHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      if (name == "ProjectName") {
        const index = e.target.selectedIndex;
        const el = e.target.childNodes[index];
        var ProjectID = el.getAttribute("ddattrprojectid");
        this.LoadMainTasksMappedByFilter(ProjectID);
      }
      if (name == "MainTaskID") {
        this.setState({
          SubTaskHandle: {
            ...this.state.SubTaskHandle,
            MainTaskName:
              e.target[e.target.selectedIndex].getAttribute("TaskName"),
          },
        });
      }
      if (name == "EstimatedDuration") {
        this.setState({
          SubTaskHandle: {
            ...this.state.SubTaskHandle,
            [name]: target.validity.valid ? value : 0,
          },
        });
      } else {
        this.setState({
          SubTaskHandle: {
            ...this.state.SubTaskHandle,
            [name]: value,
          },
        });
      }
    } catch (ex) {
      alert(ex);
    }
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  FilterSubTaskHandleChange = (e) => {
    this.setState({
      FilterSubTask: { ...this.state.FilterSubTask, TaskName: e.target.value },
    });
  };
  FilterSubTaskProjectHandleChange = (e) => {
    const index = e.target.selectedIndex;
    const el = e.target.childNodes[index];
    var _projectID = el.getAttribute("ddAttrProjectID");
    this.setState({
      ProjectNameDropdown: e.target.value,
      ProjectIDDrodown: _projectID,
    });
    this.LoadMainTasksMappedByFilter(_projectID);
  };

  FilterSubTaskMainTaskHandleChange = (e) => {
    const index = e.target.selectedIndex;
    const el = e.target.childNodes[index];
    var _MainTaskID = el.getAttribute("maintaskID");
    this.setState({
      MainTaskNameDropdown: e.target.value,
      MainTaskIDDropdown: _MainTaskID,
    });
  };
  validatePopUp = () => {
    var checkError = false;
    if (this.state.SubTaskHandle.ProjectName == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          ProjectName: "input_error",
        },
      }));
    }
    if (this.state.SubTaskHandle.MainTaskID == null) {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          MainTaskName: "input_error",
        },
      }));
    }
    if (this.state.SubTaskHandle.SubTaskName == "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          SubTaskName: "input_error",
        },
      }));
    }
    if (this.state.SubTaskHandle.SubTaskName !== "") {
      var checkTask = this.state.lstSubTask.filter(
        (SName) =>
          SName.ProjectName.trim()
            .toLowerCase()
            .includes(
              this.state.SubTaskHandle?.ProjectName.trim().toLowerCase()
            ) &&
          SName.MainTaskID == this.state.SubTaskHandle?.MainTaskID &&
          SName.TaskName.trim().toLowerCase() ==
            this.state.SubTaskHandle?.SubTaskName.trim().toLowerCase()
      );
      if (
        checkTask?.length > 0 &&
        checkTask[0]?.ID !== this.state.SubTaskHandle?.Id
      ) {
        checkError = true;
        this.setState({ Title: "SubTask Alert" });
        this.setState({
          errorMsg:
            `SubTask "` +
            this.state.SubTaskHandle?.SubTaskName +
            `" Name Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ Title: "SubTask Alert" });
          this.setState({
            errorMsg:
              `SubTask "` +
              this.state.SubTaskHandle?.SubTaskName +
              `" Name Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
    }
    if (!checkError) {
      this.AddSubTask();
    }
  };
  CheckPopUp = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CloseModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  HandelErrorRemove = (name) => {
    if (name == "ProjectName") {
      if (this.state.SubTaskHandle.ProjectName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            ProjectName: "",
          },
        }));
      }
    }
    if (name == "MainTaskID") {
      if (this.state.SubTaskHandle.MainTaskID == null) {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            MainTaskName: "",
          },
        }));
      }
    }
    if (name == "SubTaskName") {
      if (this.state.SubTaskHandle.SubTaskName == "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            SubTaskName: "",
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
                  <h3 className="m_font_heading">Sub Tasks</h3>
                </div>

                <div
                  className="col-sm-8 text-right"
                  id="ems_user_manage_projects"
                >
                  <button
                    onClick={() => {
                      this.LoadSubTasks();
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
                      this.setState({ FilterSubTaskPopup: true });
                    }}
                    type="button"
                    className="btn-black"
                    data-toggle="modal"
                    data-target="#FilterSubTask"
                    style={{
                      float: "right",
                      marginLeft: "7px",
                      marginBottom: "10px",
                    }}
                  >
                    Task Filter <i className="menu-icon fa fa-plus"></i>
                  </button>
                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={() => {
                        this.setState({
                          AddSubTaskShow: true,
                          AddSubTaskTitle: "Add SubTask",
                          AddSubTaskButton: "Add SubTask",
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
                      Add Sub Task
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
            <div className="card">
              <div className="card-body" style={{ paddingTop: "0px" }}>
                <TableContainer className="table-responsive" component={"div"}>
                  <Table className="Tabel">
                    <TableHead className="main_tbl_head">
                      <TableRow>
                        <TableCell>Project</TableCell>
                        <TableCell align="center">Main Task </TableCell>
                        <TableCell align="center">Task/PBI</TableCell>
                        {/* <TableCell align="center">Estimated Duration</TableCell>
                      <TableCell align="center">Task Type</TableCell>
                      <TableCell align="center">Phase</TableCell> */}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {this.state.lstSubTask.length > 0 ? (
                        this.state.lstSubTask
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
                                      SubTaskHandle: {
                                        Id: item.ID,
                                        SubTaskName: item.TaskName,
                                        ProjectName: item.ProjectName,
                                        MainTaskID: item.MainTaskID,
                                        MainTaskName: item.MainTask,
                                        EstimatedDuration:
                                          item.EstimatedDuration,
                                        Comments: item.Comments,
                                      },
                                      AddSubTaskShow: true,
                                      AddSubTaskTitle: "Update SubTask",
                                      AddSubTaskButton: "Update SubTask",
                                    });
                                  }
                                }}
                              >
                                <TableCell component="th" scope="row">
                                  {item.ProjectName}
                                </TableCell>
                                <TableCell align="center">
                                  {item.MainTask}
                                </TableCell>
                                <TableCell align="center">
                                  {item.TaskName}
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
                  count={this.state.lstSubTask.length}
                  page={this.state.page}
                  onChangePage={this.handleChangePage}
                  rowsPerPage={this.state.rowsPerPage}
                  onChangeRowsPerPage={this.handleChangeRowsPerPage}
                />
              </div>
            </div>
          )}
        </div>
        <AddSubTaskPopup
          AddSubTaskTitle={this.state.AddSubTaskTitle}
          AddSubtaskButton={this.state.AddSubTaskButton}
          AddSubTaskHandle={this.state.SubTaskHandle}
          lstProjectName={this.state.lstProjectName}
          lstMainTask={this.state.lstMainTask}
          AddSubTaskHandleChange={this.AddSubTaskHandleChange}
          // AddSubTaskFun={this.AddSubTask}
          AddSubTaskShow={this.state.AddSubTaskShow}
          closeAddSubTask={() => {
            this.setState({
              AddSubTaskShow: false,
              SubTaskHandle: {
                Id: 0,
                SubTaskName: "",
                ProjectName: "",
                MainTaskID: "",
              },
            });
          }}
          DeleteSubTaskFun={this.DeleteSubTask}
          errorclass={this.state.error_class}
          AddSubTaskFun={this.validatePopUp}
          HandelErrorRemove={this.HandelErrorRemove}
        />
        <FilterSubTaskPopup
          lstMainTask={this.state.lstMainTask}
          FilterSubTask={this.state.FilterSubTask}
          FilterSubTaskHandleChange={this.FilterSubTaskHandleChange}
          FilterSubTaskProjectHandleChange={
            this.FilterSubTaskProjectHandleChange
          }
          FilterSubTaskMainTaskHandleChange={
            this.FilterSubTaskMainTaskHandleChange
          }
          // ClearSearch={this.LoadSubTasks}
          FilterSubTaskFun={this.SearchSubTaskfiltervalidation}
          FilterSubTaskShow={this.state.FilterSubTaskPopup}
          lstProjectName={this.state.lstProjectName}
          ProjectIDDrodown={this.state.ProjectIDDrodown}
          ProjectNameDropdown={this.state.ProjectNameDropdown}
          MainTaskNameDropdown={this.state.MainTaskNameDropdown}
          closeFilterSubTask={() => {
            this.setState({ FilterSubTaskPopup: false });
          }}
        />
        <SureMsgPopUp
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ShowMsgPopUp}
          onConfrim={this.CheckPopUp}
          onHide={this.CloseModal}
        />
      </div>
    );
  }
}
export default SubTask;
