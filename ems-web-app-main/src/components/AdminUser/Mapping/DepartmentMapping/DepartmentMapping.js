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
import Tooltip from "@material-ui/core/Tooltip";
import Checkbox from "@material-ui/core/Checkbox";
export default class DepartmentMapping extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Loading: true,
      ShowDepartmentName: "Please Select",
      DepartmentData: null,
      DepartmentLazyLoading: {
        DepartmentScrollIndex: 1,
        DepartmentRecoardPerPage: 20,
        CurrentDepartmentDataCount: null,
        ShowDepartmentList: false,
        DepartmentHasMore: true,
      },
      DepartmentId: null,
      lstProjectNotMapped: [],
      ShowTransferList: false,
      lstDepartment: [],
      AlllstDepartment: [],
      DefaultMapMaintask: [],
      SearchDepartment: "",
      MappedList: [],
      UnmappedList: [],
      CheckBoxStatus: false,
      AdditionalMappedList: [],
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
    this.LoadDepartmentLazyLoading();
    this.SearchDepartment();
    this.LoadUserUnmappedProject();
    this.GetDefaultMaintaskList();
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
  LoadDepartmentLazyLoading = () => {
    try {
      const obj = {
        page: this.state.DepartmentLazyLoading.DepartmentScrollIndex,
        recsPerPage: this.state.DepartmentLazyLoading.DepartmentRecoardPerPage,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}DepartmentMapping/GetDepartmentLazyLoading`,
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
        this.setState({ DepartmentData: res.data.Result });
      });
    } catch (e) {
      window.location.href = "/Error";
      //alert(e);
    }
  };
  GetDefaultMaintaskList = () => {
    try {
      this.setState({ Loading: true });
      axios({
        method: "post",
        url:
          `${process.env.REACT_APP_BASE_URL}DepartmentMapping/DepartmentMappingList?CheckBoxStatus=` +
          this.state.CheckBoxStatus,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({ DefaultMapMaintask: [] }, () => {
            this.setState({
              DefaultMapMaintask: res.data.Result,
            });
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  GetDefaultMaintaskListCheckbox = () => {
    try {
      this.setState({ Loading: true });
      axios({
        method: "post",
        url:
          `${process.env.REACT_APP_BASE_URL}DepartmentMapping/DepartmentMappingList?CheckBoxStatus=` +
          this.state.CheckBoxStatus,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          this.setState({ DefaultMapMaintask: [] }, () => {
            this.setState(
              {
                DefaultMapMaintask: res.data.Result,
              },
              () => {
                this.GetselectDEpartmentList();
              }
            );
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  SearchDepartment = () => {
    try {
      this.setState({ Loading: true });
      const SearchObj = {
        MainTaskName: this.state.SearchDepartment,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Department/GetDepartments`,
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
            lstDepartment: res.data.Result,
            AlllstDepartment: res.data.Result,
          });
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      console.log(e);
    }
  };
  handleSearchDepartmentChange = (e) => {
    this.setState(
      {
        SearchDepartment: e.target.value,
      },
      () => {
        if (this.state.SearchDepartment != "") {
          var checkTask = this.state.AlllstDepartment.filter((SName) =>
            SName.DepartmentName.trim()
              .toLowerCase()
              .includes(this.state.SearchDepartment.trim().toLowerCase())
          );
          this.setState({ lstDepartment: checkTask });
          //this.SearchMainTask();
        } else {
          this.setState({
            lstDepartment: this.state.AlllstDepartment,
          });
        }
      }
    );
  };
  GetselectDEpartmentList = () => {
    try {
      this.setState({ Loading: true });
      const SearchObj = {
        DeptID: this.state.DepartmentId,
      };
      axios({
        method: "post",
        url:
          `${process.env.REACT_APP_BASE_URL}DepartmentMapping/DepartmentMapList?DeptID=` +
          this.state.DepartmentId,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode == 200) {
          var getAdditional = res.data.Result.filter(
            (Name) => Name.AdditionalCheck == true
          );
          var ArrayDefaultTask = this.state.DefaultMapMaintask;
          for (let i = 0; i < res.data.Result.length; i++) {
            ArrayDefaultTask = ArrayDefaultTask.filter(
              (SName) => SName.Id != res.data.Result[i].MaintaskID
            );
          }

          this.setState(
            {
              MappedList: res.data.Result.filter(
                (Name) => Name.AdditionalCheck == false
              ),
              DefaultMapMaintask: ArrayDefaultTask,
              AdditionalMappedList: getAdditional,
            },
            () => {
              this.setState({ Loading: false, ShowTransferList: true });
            }
          );
        }
      });
    } catch (e) {
      console.log(e);
    }
  };
  _onSelect = (e) => {
    this.GetDefaultMaintaskList();
    var _DepartmentID = e.target.getAttribute("ddAttrDepartmentID");
    var value = e.target.value;
    var _DepartmentName = e.target.getAttribute("ddAttrDepartmenName");
    if (_DepartmentID != null) {
      this.setState(
        { DepartmentId: _DepartmentID, ShowTransferList: false },
        () => {
          this.GetselectDEpartmentList();
          this.setState({
            //ShowMainTaskName: value,
            UnmappedList: [],
            MappedList: [],
            ShowDepartmentName: _DepartmentName,
          });
        }
      );
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
        this.setState({
          ProjectList: res.data.Result,
        });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  render() {
    var DepartmentDataList = null;
    DepartmentDataList =
      this.state.DepartmentData !== null
        ? this.state.DepartmentData.map((item) => {
            return (
              <option
                ddAttrDepartmentID={item.ID}
                value={item.DepartmentName}
                ddAttrDepartmenName={item.DepartmentName}
                onClick={this._onSelect}
              >
                {item.DepartmentName}
              </option>
            );
          })
        : "";
    var SearchDepartmentList = null;
    SearchDepartmentList =
      this.state.lstDepartment !== null
        ? this.state.lstDepartment.map((item) => {
            return (
              <li
                onClick={this._onSelect}
                ddAttrDepartmentID={item.ID}
                ddAttrDepartmenName={item.DepartmentName}
                value={item.DepartmentName}
              >
                {item.DepartmentName}
              </li>
            );
          })
        : "";
    return (
      <div>
        <div className="content mt-3 ems_departure">
          <div className="row">
            <div className="col-sm-12">
              <div className="row rpt_mbl_header">
                <div
                  className="col-sm-4 text-left"
                  style={{ marginBottom: "10px" }}
                >
                  <h3 className="m_font_heading">Department Mapping</h3>
                </div>
                {this.state.Loading ? (
                  <Loader />
                ) : (
                  <div className="col-sm-8 text-right">
                    <Tooltip
                      className="showing_check"
                      title="Show Additional Task"
                      arrow
                    >
                      <label>
                        <Checkbox
                          onClick={() => {
                            if (this.state.CheckBoxStatus == true) {
                              this.setState(
                                {
                                  CheckBoxStatus: false,
                                  ShowTransferList: false,
                                  //DefaultMapMaintask: null,
                                },
                                () => {
                                  this.GetDefaultMaintaskListCheckbox();
                                  //this.GetselectDEpartmentList();
                                }
                              );
                            } else {
                              this.setState(
                                {
                                  CheckBoxStatus: true,
                                  ShowTransferList: false,
                                  //DefaultMapMaintask: null,
                                },
                                () => {
                                  this.GetDefaultMaintaskListCheckbox();

                                  //
                                }
                              );
                            }
                          }}
                          checked={this.state.CheckBoxStatus}
                        />
                        Show Additional Task
                      </label>
                    </Tooltip>
                    <div className="input_list transfer_list_search">
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Search"
                        onChange={this.handleSearchDepartmentChange}
                        value={
                          this.state.SearchDepartment == null
                            ? ""
                            : this.state.SearchDepartment
                        }
                      />
                      {this.state.SearchDepartment != "" ? (
                        this.state.lstDepartment != null ? (
                          this.state.lstDepartment.length > 0 ? (
                            <div className="input_box">
                              <ul>{SearchDepartmentList}</ul>
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
                          {this.state.ShowDepartmentName}
                        </Dropdown.Toggle>

                        <Dropdown.Menu className="align_top dashboard_dropdown">
                          <Dropdown.Item>
                            <div
                              className="custom_select_latest"
                              id="scrollableDiv"
                            >
                              <InfiniteScroll
                                dataLength={DepartmentDataList.length} //This is important field to render the next data
                                next={() => {
                                  this.setState(
                                    {
                                      DepartmentLazyLoading: {
                                        ...this.state.DepartmentLazyLoading,
                                        DepartmentRecoardPerPage:
                                          this.state.DepartmentLazyLoading
                                            .DepartmentRecoardPerPage + 20,
                                        CurrentDepartmentDataCount:
                                          DepartmentDataList.length,
                                      },
                                    },
                                    () => {
                                      if (
                                        this.state.DepartmentLazyLoading
                                          .CurrentDepartmentDataCount +
                                          20 ==
                                        this.state.DepartmentLazyLoading
                                          .DepartmentRecoardPerPage
                                      ) {
                                        this.LoadDepartmentLazyLoading();
                                      } else {
                                        this.setState(
                                          {
                                            DepartmentLazyLoading: {
                                              ...this.state
                                                .DepartmentLazyLoading,
                                              DepartmentHasMore: false,
                                            },
                                          },
                                          this.LoadDepartmentLazyLoading()
                                        );
                                      }
                                    }
                                  );
                                }}
                                hasMore={
                                  this.state.DepartmentLazyLoading
                                    .DepartmentHasMore
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
                                {DepartmentDataList}
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
                      lstDepartmentMapped={this.state.MappedList}
                      lstDepartmentNotMapped={this.state.DefaultMapMaintask}
                      CheckComponent="DepartmentMapped"
                      DepartmentId={this.state.DepartmentId}
                      CheckBoxStatus={this.state.CheckBoxStatus}
                      AdditionalMappedList={this.state.AdditionalMappedList}
                    />
                  ) : (
                    <h2 className="mapping_placeholder">Map Department</h2>
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
