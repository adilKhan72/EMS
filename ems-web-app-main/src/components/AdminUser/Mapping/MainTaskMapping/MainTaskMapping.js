import React, { Component } from "react";
import { Dropdown } from "react-bootstrap";
import "react-dropdown/style.css";
import InfiniteScroll from "react-infinite-scroll-component";
//import { Form, InputGroup, Col, Button } from "react-bootstrap";
import { ListGroup } from "react-bootstrap";
import "../ProjectMapping.css";
import axios from "axios";
import Transferlist from "../TransferList/Transferlist";
import Loader from "../../../Loader/Loader";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "react-crypt-gsm";
export default class MainTaskMapping extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Loading: true,
      ShowMainTaskName: "Please Select",
      MainTaskData: null,
      ProjectLazyLoading: {
        ProjectScrollIndex: 1,
        ProjectRecoardPerPage: 20,
        CurrentProjectDataCount: null,
        ShowProjectList: false,
        ProjectHasMore: true,
      },
      MainTaskId: null,
      lstMappedMaintasks: [],
      lstProjectList: [],
      lstMappedProjectMaintask: [],
      lstProjectNotMapped: [],
      ShowTransferList: false,
      lstSearchMainTask: [],
      AlllstSearchMainTask: [],
      SearchMainTask: "",
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
    this.LoadMainTaskLazyLoading();
    this.LoadProjectName();
    this.SearchMainTask();
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
  LoadMainTaskLazyLoading = () => {
    try {
      const obj = {
        page: this.state.ProjectLazyLoading.ProjectScrollIndex,
        recsPerPage: this.state.ProjectLazyLoading.ProjectRecoardPerPage,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetMainTaskLazyLoading`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: obj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ MainTaskData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  LoadProjectName = () => {
    try {
      this.setState({ Loading: true });
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
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstProjectList: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadProjectMappedMaintask = () => {
    try {
      this.setState({ Loading: true });
      this.setState({ ShowTransferList: false });
      const MaintaskId = {
        MaintaskId: this.state.MainTaskId,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/FetchProjectMappedMainTask`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: MaintaskId,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState(
          {
            lstMappedProjectMaintask: res.data.Result,
            SearchMainTask: null,
            lstSearchMainTask: [],
          },
          () => {
            this.getNotMappedProject();
          }
        );
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SearchMainTask = () => {
    try {
      this.setState({ Loading: true });
      const SearchObj = {
        MainTaskName: this.state.SearchMainTask,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/SearchMainTasks`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: SearchObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({
            lstSearchMainTask: res.data.Result,
            AlllstSearchMainTask: res.data.Result,
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  getNotMappedProject = () => {
    var temp = this.state.lstProjectList;
    if (this.state.lstMappedProjectMaintask.length > 0) {
      this.state.lstMappedProjectMaintask.map((item) => {
        var ProjectId = item.ProjectId;
        this.state.lstProjectList.map((project) => {
          if (ProjectId == project.ID) {
            var index = temp.indexOf(project);
            delete temp[index];
          }
        });
      });
    }
    var filteredArray = temp.filter(function (el) {
      return el != null;
    });
    this.setState({ lstProjectNotMapped: filteredArray }, () => {
      this.setState({ ShowTransferList: true });
      this.LoadProjectName();
    });
    console.log(filteredArray);
    console.log(this.state.lstProjectNotMapped);
  };
  _onSelect = (e) => {
    var _MainTaskID = e.target.getAttribute("ddAttrMainTaskID");
    if (_MainTaskID != null) {
      this.setState(
        {
          MainTaskId: _MainTaskID,
          ShowMainTaskName: e.target.value,
        },
        () => {
          this.LoadProjectMappedMaintask();
        }
      );
    }
  };
  handleSearchMainTaskChange = (e) => {
    this.setState(
      {
        SearchMainTask: e.target.value,
      },
      () => {
        if (this.state.SearchMainTask != "") {
          var checkTask = this.state.AlllstSearchMainTask.filter((SName) =>
            SName.MainTaskName.trim()
              .toLowerCase()
              .includes(this.state.SearchMainTask.trim().toLowerCase())
          );
          this.setState({ lstSearchMainTask: checkTask });
          //this.SearchMainTask();
        } else {
          this.setState({ lstSearchMainTask: this.state.AlllstSearchMainTask });
        }
      }
    );
  };
  _onSearchSelect = (e) => {
    var _MainTaskID = e.target.getAttribute("ddAttrMainTaskID");
    var _MainTaskName = e.target.getAttribute("ddAttrMainTask");
    if (_MainTaskID !== null) {
      this.setState(
        {
          MainTaskId: _MainTaskID,
          SearchMainTask: _MainTaskName,
          ShowMainTaskName: _MainTaskName,
        },
        () => {
          this.LoadProjectMappedMaintask();
          // /this.SearchMainTask();
        }
      );
    }
  };
  render() {
    var ProjectNameList = null;
    ProjectNameList =
      this.state.MainTaskData !== null
        ? this.state.MainTaskData.map((item) => {
            return (
              <option
                ddAttrMainTaskID={item.Id}
                value={item.MainTaskName}
                onClick={this._onSelect}
              >
                {item.MainTaskName}
              </option>
            );
          })
        : "";

    var SearchMainTaskNameList = null;
    SearchMainTaskNameList =
      this.state.lstSearchMainTask !== null
        ? this.state.lstSearchMainTask.map((item) => {
            return (
              <li
                onClick={this._onSearchSelect}
                ddAttrMainTaskID={item.Id}
                ddAttrMainTask={item.MainTaskName}
              >
                {item.MainTaskName}
              </li>
            );
          })
        : "";
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
                  <h3 className="m_font_heading">Main Task Mapping</h3>
                </div>
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="col-sm-8 text-right">
                    <div className="input_list transfer_list_search">
                      <input
                        type="text"
                        className="form-control"
                        onChange={this.handleSearchMainTaskChange}
                        value={
                          this.state.SearchMainTask == null
                            ? ""
                            : this.state.SearchMainTask
                        }
                        placeholder="Search"
                      />
                      {this.state.SearchMainTask != "" ? (
                        this.state.lstSearchMainTask != null ? (
                          this.state.lstSearchMainTask.length > 0 ? (
                            <div className="input_box">
                              <ul>{SearchMainTaskNameList}</ul>
                            </div>
                          ) : null
                        ) : null
                      ) : null}
                    </div>
                    <div
                      class="custom_select_latest_outer"
                      id="ems_task_mapping"
                    >
                      <Dropdown
                        style={{ maxWidth: "200px", float: "right" }}
                        menuAlign="left"
                      >
                        <Dropdown.Toggle
                          id="dropdown-basic"
                          className="btn-black mr-1 project_list_dropdwon"
                        >
                          {this.state.ShowMainTaskName}
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top dashboard_dropdown">
                          <Dropdown.Item>
                            <div
                              className="custom_select_latest"
                              id="scrollableDiv"
                            >
                              <InfiniteScroll
                                dataLength={ProjectNameList.length} //This is important field to render the next data
                                next={() => {
                                  this.setState(
                                    {
                                      ProjectLazyLoading: {
                                        ...this.state.ProjectLazyLoading,
                                        ProjectRecoardPerPage:
                                          this.state.ProjectLazyLoading
                                            .ProjectRecoardPerPage + 20,
                                        CurrentProjectDataCount:
                                          ProjectNameList.length,
                                      },
                                    },
                                    () => {
                                      if (
                                        this.state.ProjectLazyLoading
                                          .CurrentProjectDataCount +
                                          20 ==
                                        this.state.ProjectLazyLoading
                                          .ProjectRecoardPerPage
                                      ) {
                                        this.LoadMainTaskLazyLoading();
                                      } else {
                                        this.setState(
                                          {
                                            ProjectLazyLoading: {
                                              ...this.state.ProjectLazyLoading,
                                              ProjectHasMore: false,
                                            },
                                          },
                                          this.LoadMainTaskLazyLoading()
                                        );
                                      }
                                    }
                                  );
                                }}
                                hasMore={
                                  this.state.ProjectLazyLoading.ProjectHasMore
                                }
                                loader={
                                  <p className="text-center mt-3">
                                    <i class="fa fa-spinner fa-pulse"></i>{" "}
                                    Loading ...
                                  </p>
                                }
                                scrollableTarget="scrollableDiv"
                                endMessage={
                                  <p
                                    style={{
                                      textAlign: "center",
                                      lineHeight: "40px",
                                    }}
                                  >
                                    <b>Yay! You have seen it all</b>
                                  </p>
                                }
                              >
                                {/* <option
                                ddAttrMainTaskID="-1"
                                value="All MainTask"
                                onClick={this._onSelect}
                              >
                                All Project
                              </option> */}
                                {ProjectNameList}
                              </InfiniteScroll>
                            </div>
                          </Dropdown.Item>
                        </Dropdown.Menu>
                      </Dropdown>
                    </div>
                    {/*    <button className="btn-black mr-1 mapping_btn" type="submit">
                    Submit
                  </button> */}
                  </div>
                )}
              </div>
            </div>
          </div>
          <div className="card card-padding my-4" id="ems_client_mapping">
            <div className="card-body transfer_list">
              <div className="row no-guttters">
                <div className="col-md-12">
                  {this.state.ShowTransferList == true ? (
                    <Transferlist
                      lstProjectMapped={this.state.lstMappedProjectMaintask}
                      lstProjectNotMapped={this.state.lstProjectNotMapped}
                      CheckComponent="MainTaskMapped"
                      MainTaskId={this.state.MainTaskId}
                    />
                  ) : (
                    <h2 className="mapping_placeholder">Map Your MainTask</h2>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
