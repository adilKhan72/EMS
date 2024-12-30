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
export default class ProjectMapping extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Loading: true,
      ShowProjectName: "Please Select",
      ProjectData: null,
      ProjectLazyLoading: {
        ProjectScrollIndex: 1,
        ProjectRecoardPerPage: 20,
        CurrentProjectDataCount: null,
        ShowProjectList: false,
        ProjectHasMore: true,
      },
      lstProjectUser: [],
      lstUserMapped: [],
      lstUser: [],
      ProjectId: null,
      ShowTransferList: false,
      SearchProject: "",
      lstSearchProject: null,
      AlllstSearchProject: null,
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
    this.LoadProjectNameLazyLoading();
    this.LoadAllUser();
    this.SearchProject();
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
  LoadProjectNameLazyLoading = () => {
    try {
      const obj = {
        page: this.state.ProjectLazyLoading.ProjectScrollIndex,
        recsPerPage: this.state.ProjectLazyLoading.ProjectRecoardPerPage,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/GetProjectsLazyLoading`,
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
        this.setState({ ProjectData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  LoadProjectUser = () => {
    try {
      this.setState({ Loading: true });
      this.setState({ ShowTransferList: false });
      const ProjectId = {
        ProjectId: this.state.ProjectId,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/FetchProjectUser`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: ProjectId,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState(
          {
            lstProjectUser: res.data.Result,
            SearchProject: null,
            lstSearchProject: null,
          },
          () => {
            this.getMappedUser();
          }
        );
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadAllUser = () => {
    try {
      this.setState({ Loading: true });
      const UserIdObj = {
        UserID: 0,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/GetUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserIdObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        var GetActive = res.data.Result.filter((SName) => SName.Status == true);
        this.setState({ lstUser: GetActive });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  SearchProject = () => {
    try {
      this.setState({ Loading: true });
      const SearchObj = {
        ProjectName: this.state.SearchProject,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Projects/SearchProject`,
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
            lstSearchProject: res.data.Result,
            AlllstSearchProject: res.data.Result,
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  getMappedUser = () => {
    var temp = this.state.lstUser;
    if (this.state.lstProjectUser.length > 0) {
      this.state.lstProjectUser.map((item) => {
        var UserID = item.UserId;
        this.state.lstUser.map((user) => {
          if (UserID == user.UserProfileTableID) {
            var index = temp.indexOf(user);
            delete temp[index];
          }
        });
      });
    }
    var filteredArray = temp.filter(function (el) {
      return el != null;
    });
    this.setState({ lstUserMapped: filteredArray }, () => {
      this.setState({ ShowTransferList: true });
    });
    console.log(filteredArray);
    console.log(this.state.lstProjectUser);
  };
  _onSelect = (e) => {
    var _projectID = e.target.getAttribute("ddAttrProjectID");
    if (_projectID != null) {
      this.setState(
        {
          ProjectId: _projectID,
          ShowProjectName: e.target.value,
        },
        () => {
          this.LoadProjectUser();
          this.LoadAllUser();
        }
      );
    }
  };
  handleSearchProjectChange = (e) => {
    this.setState(
      {
        SearchProject: e.target.value,
      },
      () => {
        if (this.state.SearchProject != "") {
          var checkproject = this.state.AlllstSearchProject.filter((SName) =>
            SName.ProjectName?.trim()
              .toLowerCase()
              .includes(this.state.SearchProject?.trim().toLowerCase())
          );
          this.setState({ lstSearchProject: checkproject });
        } else {
          this.setState({ lstSearchProject: this.state.AlllstSearchProject });
        }
        //this.SearchProject();
      }
    );
  };
  _onSearchSelect = (e) => {
    var _projectID = e.target.getAttribute("ddAttrProjectID");
    var _projectName = e.target.getAttribute("ddAttrProjectName");
    if (_projectID !== null) {
      this.LoadAllUser();
      this.setState(
        {
          ProjectId: _projectID,
          SearchProject: _projectName,
          ShowProjectName: _projectName,
        },
        () => {
          //this.SearchProject();
          this.LoadProjectUser();
        }
      );
    }
  };
  render() {
    var SearchProjectNameList = null;
    SearchProjectNameList =
      this.state.lstSearchProject !== null
        ? this.state.lstSearchProject.map((item) => {
            return item.isActive == false ? null : (
              <li
                onClick={this._onSearchSelect}
                ddAttrProjectID={item.ProjectID}
                ddAttrProjectName={item.ProjectName}
              >
                {item.ProjectName}
              </li>
            );
          })
        : "";
    var ProjectNameList = null;
    ProjectNameList =
      this.state.ProjectData !== null
        ? this.state.ProjectData.map((item) => {
            return item.IsActive == false ? null : (
              <option
                ddAttrProjectID={item.ID}
                ddAttr={item.ProjectDescription}
                value={item.Name}
                onClick={this._onSelect}
              >
                {item.Name}
              </option>
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
                  <h3 className="m_font_heading">Project Mapping</h3>
                </div>
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="col-sm-8 text-right">
                    <div className="input_list transfer_list_search">
                      <input
                        type="text"
                        className="form-control"
                        onChange={this.handleSearchProjectChange}
                        value={
                          this.state.SearchProject == null
                            ? ""
                            : this.state.SearchProject
                        }
                        placeholder="Search"
                      />
                      {this.state.SearchProject != "" ? (
                        this.state.lstSearchProject != null ? (
                          this.state.lstSearchProject.length > 0 ? (
                            <div className="input_box">
                              <ul>{SearchProjectNameList}</ul>
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
                          {this.state.ShowProjectName}
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
                                        this.LoadProjectNameLazyLoading();
                                      } else {
                                        this.setState(
                                          {
                                            ProjectLazyLoading: {
                                              ...this.state.ProjectLazyLoading,
                                              ProjectHasMore: false,
                                            },
                                          },
                                          this.LoadProjectNameLazyLoading()
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
                                ddAttrProjectID="-1"
                                value="All Project"
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
                    {/*  <button className="btn-black mr-1 mapping_btn" type="submit">
                    Submit
                  </button> */}

                    {/* <Form className="mapping_form">
                    <Form.Row className="align-items-center">
                      <div className="my-1 mapping_select">
                        <Form.Label htmlFor="inlineFormInputName" srOnly>
                          Name
                        </Form.Label>
                        <div className="styled-select">
                          <Form.Control as="select">
                            <option>Rezaid EMS</option>
                            <option>link Shortner</option>
                            <option>CMS</option>
                          </Form.Control>
                        </div>
                      </div>

                      <div className="my-1 mapping_btn">
                        <Button type="submit" className="btn-black mr-1">
                          Submit
                        </Button>
                      </div>
                    </Form.Row>
                  </Form> */}
                  </div>
                )}
              </div>
            </div>
          </div>
          <div className="card card-padding my-4" id="ems_client_mapping">
            <div className="card-body transfer_list">
              <div className="row no-gutters">
                <div className="col-md-12">
                  {this.state.ShowTransferList == true ? (
                    <>
                      <Transferlist
                        lstProjectUser={this.state.lstProjectUser}
                        lstUserMapped={this.state.lstUserMapped}
                        CheckComponent="ProjectMapped"
                        ProjectID={this.state.ProjectId}
                        ShowProjectName={this.state.ShowProjectname}
                      />
                    </>
                  ) : (
                    <h2 className="mapping_placeholder">Map Your Project</h2>
                  )}
                </div>
              </div>
            </div>
          </div>
          {/* <div
            className="card card-padding"
            style={{ maxWidth: "770px", margintop: "20px" }}
          >
            <div className="card-body">
              <div className="row">
                <div className="col-sm-6">
                  <h5 className="mb-2">Main Projects</h5>
                  <ListGroup as="ul">
                    <ListGroup.Item>Cras justo odio</ListGroup.Item>
                    <ListGroup.Item as="li">
                      Dapibus ac facilisis in
                    </ListGroup.Item>
                    <ListGroup.Item as="li" disabled>
                      Morbi leo risus
                    </ListGroup.Item>
                    <ListGroup.Item as="li">
                      Porta ac consectetur ac
                    </ListGroup.Item>
                    <ListGroup.Item>Cras justo odio</ListGroup.Item>
                    <ListGroup.Item as="li">
                      Dapibus ac facilisis in
                    </ListGroup.Item>
                    <ListGroup.Item as="li" disabled>
                      Morbi leo risus
                    </ListGroup.Item>
                    <ListGroup.Item as="li">
                      Porta ac consectetur ac
                    </ListGroup.Item>
                  </ListGroup>
                </div>
                <div className="col-sm-6 right-box">
                  <h5 className="mb-2">Drop Project</h5>
                  <ListGroup as="ul">
                    <ListGroup.Item>Cras justo odio</ListGroup.Item>
                    <ListGroup.Item as="li">
                      Dapibus ac facilisis in
                    </ListGroup.Item>
                    <ListGroup.Item as="li" disabled>
                      Morbi leo risus
                    </ListGroup.Item>
                    <ListGroup.Item as="li">
                      Porta ac consectetur ac
                    </ListGroup.Item>
                    <ListGroup.Item>Cras justo odio</ListGroup.Item>
                    <ListGroup.Item as="li">
                      Dapibus ac facilisis in
                    </ListGroup.Item>
                    <ListGroup.Item as="li" disabled>
                      Morbi leo risus
                    </ListGroup.Item>
                    <ListGroup.Item as="li">
                      Porta ac consectetur ac
                    </ListGroup.Item>
                  </ListGroup>
                </div>
              </div>
            </div>
          </div> */}
        </div>
      </div>
    );
  }
}
