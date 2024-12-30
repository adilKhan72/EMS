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
export default class ClientMapping extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Loading: true,
      ShowClientName: "Please Select",
      ClientData: null,
      ProjectLazyLoading: {
        ProjectScrollIndex: 1,
        ProjectRecoardPerPage: 20,
        CurrentProjectDataCount: null,
        ShowProjectList: false,
        ProjectHasMore: true,
      },
      ClientId: null,
      lstMappedMaintasks: [],
      lstProjectList: [],
      lstMappedProjectClient: [],
      lstProjectNotMapped: [],
      ShowTransferList: false,
      lstSearchClient: [],
      AlllstSearchClient: [],
      SearchClient: "",
      MappedList: [],
      UnmappedList: [],
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
    this.setState({ Loading: false, ShowTransferList: false });
    this.LoadClientLazyLoading();
    this.SearchClients();
    this.LoadUserUnmappedProject();
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
  LoadClientLazyLoading = () => {
    try {
      const obj = {
        page: this.state.ProjectLazyLoading.ProjectScrollIndex,
        recsPerPage: this.state.ProjectLazyLoading.ProjectRecoardPerPage,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}MainTask/GetClientLazyLoading`,
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
        this.setState({ ClientData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  SearchClients = () => {
    try {
      this.setState({ Loading: true });
      const SearchObj = {
        MainTaskName: this.state.SearchClient,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Client/GetClientList`,
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
            lstSearchClient: res.data.Result,
            AlllstSearchClient: res.data.Result,
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  handleSearchClientChange = (e) => {
    this.setState(
      {
        SearchClient: e.target.value,
      },
      () => {
        if (this.state.SearchClient != "") {
          var checkTask = this.state.AlllstSearchClient.filter((SName) =>
            SName.ClientName.trim()
              .toLowerCase()
              .includes(this.state.SearchClient.trim().toLowerCase())
          );
          this.setState({ lstSearchClient: checkTask });
          //this.SearchMainTask();
        } else {
          this.setState({ lstSearchClient: this.state.AlllstSearchClient });
        }
      }
    );
  };
  _onSelect = (e) => {
    var _ClientID = e.target.getAttribute("ddAttrClientID");
    var value = e.target.value;
    var _ClientName = e.target.getAttribute("ddAttrClientName");
    if (_ClientID != null) {
      this.setState({ ShowTransferList: false }, () => {
        this.setState(
          {
            ClientId: _ClientID,
            ShowMainTaskName: value,
            UnmappedList: [],
            MappedList: [],
            ShowClientName: _ClientName,
          },
          () => {
            var CheckMappedProject = this.state.ProjectList.filter(
              (SName) => SName.ClientID == _ClientID
            );
            if (CheckMappedProject.length > 0) {
              this.setState({ MappedList: CheckMappedProject });
            }
            var CheckUnMappedProject = this.state.ProjectList.filter(
              (SName) =>
                SName.ClientID != _ClientID &&
                (SName.ClientID == null || SName.ClientID == 0)
            );
            if (CheckUnMappedProject.length > 0) {
              this.setState({
                UnmappedList: CheckUnMappedProject,
                ShowTransferList: true,
              });
            }
            this.setState({
              ShowTransferList: true,
            });
          }
        );
      });
    }
  };

  LoadUserProject = () => {
    try {
      const UserIdObj = {
        UserID: this.state.ClientId,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/GetResourceMappingList`,
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
        this.setState({
          lstMappedProjectClient: res.data.Result,
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  LoadUserUnmappedProject = () => {
    try {
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/ClientunMapping`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        // var CheckMappedProject = res.data.Result.filter(SName => SName.ClientID === 19);
        // var CheckMappedProjects = res.data.Result.filter(SName => SName.ClientID !== 19 && (SName.ClientID == null || SName.ClientID === 0));
        this.setState({
          ProjectList: res.data.Result,
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  render() {
    var ClientList = null;
    ClientList =
      this.state.ClientData !== null
        ? this.state.ClientData.map((item) => {
            return (
              <option
                ddAttrClientID={item.ID}
                value={item.ClientName}
                ddAttrClientName={item.ClientName}
                onClick={this._onSelect}
              >
                {item.ClientName}
              </option>
            );
          })
        : "";
    var SearchClientList = null;
    SearchClientList =
      this.state.lstSearchClient !== null
        ? this.state.lstSearchClient.map((item) => {
            return (
              <li
                onClick={this._onSelect}
                ddAttrClientID={item.ID}
                ddAttrClientName={item.ClientName}
                value={item.ClientName}
              >
                {item.ClientName}
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
                  <h3 className="m_font_heading">Client Mapping</h3>
                </div>
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="col-sm-8 text-right">
                    <div className="input_list transfer_list_search">
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Search"
                        onChange={this.handleSearchClientChange}
                        value={
                          this.state.SearchClient == null
                            ? ""
                            : this.state.SearchClient
                        }
                      />
                      {this.state.SearchClient != "" ? (
                        this.state.lstSearchClient != null ? (
                          this.state.lstSearchClient.length > 0 ? (
                            <div className="input_box">
                              <ul>{SearchClientList}</ul>
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
                          {this.state.ShowClientName}
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top dashboard_dropdown">
                          <Dropdown.Item>
                            <div
                              className="custom_select_latest"
                              id="scrollableDiv"
                            >
                              <InfiniteScroll
                                dataLength={ClientList.length} //This is important field to render the next data
                                next={() => {
                                  this.setState(
                                    {
                                      ProjectLazyLoading: {
                                        ...this.state.ProjectLazyLoading,
                                        ProjectRecoardPerPage:
                                          this.state.ProjectLazyLoading
                                            .ProjectRecoardPerPage + 20,
                                        CurrentProjectDataCount:
                                          ClientList.length,
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
                                        this.LoadClientLazyLoading();
                                      } else {
                                        this.setState(
                                          {
                                            ProjectLazyLoading: {
                                              ...this.state.ProjectLazyLoading,
                                              ProjectHasMore: false,
                                            },
                                          },
                                          this.LoadClientLazyLoading()
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
                                {ClientList}
                              </InfiniteScroll>
                            </div>
                          </Dropdown.Item>
                        </Dropdown.Menu>
                      </Dropdown>
                    </div>
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
                    <Transferlist
                      lstClientProjectMapped={this.state.MappedList}
                      lstClientProjectNotMapped={this.state.UnmappedList}
                      CheckComponent="ClientMapped"
                      ClientId={this.state.ClientId}
                    />
                  ) : (
                    <h2 className="mapping_placeholder">Map Your Client</h2>
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
