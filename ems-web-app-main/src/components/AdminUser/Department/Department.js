import React, { Component } from "react";
import axios from "axios";
import Cookies from "js-cookie";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import { encrypt, decrypt } from "react-crypt-gsm";
import AddUpdateDepartment from "./AddUpdateDepartment";
import TablePagination from "@material-ui/core/TablePagination";
import DepartmentFilter from "./DepartmentFilter";
import Loader from "../../Loader/Loader";
import PopUpMsgDepartment from "./PopUpMsgDepartment";
class Department extends Component {
  constructor(props) {
    super(props);
    this.state = {
      lstDepartment: [],
      lstDepartmentAll: [],
      page: 0,
      rowsPerPage: 10,
      AddupdateDepartmentModel: false,
      Title: null,
      error_class: {
        DepartmentName: "",
        DepartmentIsActive: "",
      },
      DepartmentHandle: {
        ID: 0,
        DepartmentName: "",
        DepartmentIsActive: "",
      },
      AddDepartButton: "Save",
      DepartmentFilterstate: false,
      SelectedDepartment: -1,
      FilterStatus: null,
      Loading: false,
      show: false,
      Title: null,
      ShowMsgs: null,
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
    this.LoadDepartments();
  }
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };

  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  LoadDepartments = () => {
    try {
      this.setState({ Loading: true });
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Department/GetDepartments`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstDepartment: res.data.Result,
          lstDepartmentAll: res.data.Result,
        });

        if (this.state.SearchClientvalue != null) {
          this.FilterUpdateClient();
        }
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  DeleteRecord = () => {
    try {
      this.setState({ Loading: true });
      const DelDepart = {
        ID: this.state.DepartmentHandle.ID,
      };

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Department/DeleteDepartment`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DelDepart,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState(
            { AddupdateDepartmentModel: false, Loading: false },
            () => {
              this.LoadDepartments();
            }
          );
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddupdateDepartmentModel = () => {
    this.setState({ AddupdateDepartmentModel: true, Title: "Add Department" });
    this.setState({
      DepartmentHandle: {
        ...this.state.DepartmentHandle,
        DepartmentIsActive: "Active",
      },
    });
  };
  DepartmentFilterModel = () => {
    this.setState({ DepartmentFilterstate: true });
  };
  closeAddupdateDepartment = () => {
    this.setState({ AddupdateDepartmentModel: false, Title: null });
    this.setState({
      DepartmentHandle: {
        ID: 0,
        DepartmentName: "",
        DepartmentIsActive: "",
      },
    });
  };
  HandelErrorRemove = (name) => {
    if (name === "DepartmentName") {
      if (this.state.DepartmentHandle.DepartmentName === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            DepartmentName: "",
          },
        }));
      }
    }
  };
  AddDepartmentHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        DepartmentHandle: {
          ...this.state.DepartmentHandle,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  SearchDepartmentGrid = () => {
    this.setState({
      Loading: true,
    });
    var getdeplist = [];
    var filter_status =
      this.state.FilterStatus == "true"
        ? true
        : this.state.FilterStatus == "false"
        ? false
        : null;
    if (this.state.SelectedDepartment != -1 && filter_status == null) {
      getdeplist = this.state.lstDepartmentAll.filter(
        (SName) => SName.ID == this.state.SelectedDepartment
      );
    }
    if (this.state.SelectedDepartment == -1 && filter_status != null) {
      getdeplist = this.state.lstDepartmentAll.filter(
        (SName) => SName.IsActive == filter_status
      );
    }
    if (this.state.SelectedDepartment != -1 && filter_status != null) {
      getdeplist = this.state.lstDepartmentAll.filter(
        (SName) =>
          SName.ID == this.state.SelectedDepartment &&
          SName.IsActive == filter_status
      );
    }
    this.setState({
      lstDepartment: getdeplist,
      DepartmentFilterstate: false,
      Loading: false,
    });
  };
  validatePopUp = () => {
    var checkError = false;
    if (this.state.DepartmentHandle.DepartmentName === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          DepartmentName: "input_error",
        },
      }));
    }
    if (this.state.DepartmentHandle.DepartmentIsActive === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          DepartmentIsActive: "input_error",
        },
      }));
    }
    if (!checkError && this.state.DepartmentHandle.ID == 0) {
      var checkTask = this.state.lstDepartmentAll.filter(
        (Name) =>
          Name.DepartmentName.trim().toLowerCase() ==
          this.state.DepartmentHandle.DepartmentName.trim().toLowerCase()
      );
      if (checkTask.length > 0) {
        checkError = true;
        this.setState({
          show: true,
          Title: "Department Alert",
          ShowMsgs:
            `Department "` +
            this.state.DepartmentHandle.DepartmentName +
            `" Name Already exit`,
        });
      }
    }
    if (!checkError && this.state.DepartmentHandle.ID > 0) {
      var checkTask = this.state.lstDepartmentAll.filter(
        (Name) =>
          Name.DepartmentName.trim().toLowerCase() ==
            this.state.DepartmentHandle.DepartmentName.trim().toLowerCase() &&
          Name.ID != this.state.DepartmentHandle.ID
      );
      if (checkTask.length > 0) {
        checkError = true;
        this.setState({
          show: true,
          Title: "Department Alert",
          ShowMsgs:
            `Department "` +
            this.state.DepartmentHandle.DepartmentName +
            `" Name Already exit`,
        });
      }
    }
    if (!checkError) {
      this.AddDepartment();
    }
  };

  AddDepartment = () => {
    try {
      this.setState({ Loading: true });

      var AddUpdateDepartment = {
        ID: 0,
        DepartmentName: "",
        IsActive: 0,
      };

      if (this.state.DepartmentHandle.ID === 0) {
        AddUpdateDepartment = {
          ID: 0,
          DepartmentName: this.state.DepartmentHandle.DepartmentName,
          IsActive:
            this.state.DepartmentHandle.DepartmentIsActive === "Active" ? 1 : 0,
        };
      } else {
        AddUpdateDepartment = {
          ID: this.state.DepartmentHandle.ID,
          DepartmentName: this.state.DepartmentHandle.DepartmentName,
          IsActive:
            this.state.DepartmentHandle.DepartmentIsActive === "Active" ? 1 : 0,
        };
      }

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Department/InsertDepartment`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddUpdateDepartment,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState({
            DepartmentHandle: {
              ID: 0,
              DepartmentName: "",
              DepartmentIsActive: "",
            },
          });
          // this.setState({
          //   ClientHandle: {
          //     ClientName: "",
          //     ClientEmail: "",
          //     ClientContact: "",
          //     ClientAddress: "",
          //     ClientWebsite: "",
          //     ClientFacebook: "",
          //     ClientTwitter: "",
          //     ClientInstagram: "",
          //     Status: "",
          //     Id: 0,
          //   },
          // });
          this.setState(
            { AddupdateDepartmentModel: false, Loading: false },
            () => {
              this.LoadDepartments();
            }
          );
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
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
  render() {
    return this.state.Loading ? (
      <Loader />
    ) : (
      <div id="ems_client_view">
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="row rpt_mbl_header">
                <div
                  className="col-sm-4 text-left"
                  style={{ marginBottom: "10px" }}
                >
                  <h3 className="m_font_heading">Manage Departments</h3>
                </div>

                <div
                  className="col-sm-8 text-right"
                  id="ems_user_manage_clients"
                >
                  {/* <div className="input_list transfer_list_search">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Search"
                      onChange={this.FilterClient}
                      value={
                        this.state.SearchClient == null
                          ? ""
                          : this.state.SearchClient
                      }
                    />
                  </div> */}
                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <div
                      class="custom_select_latest_outer"
                      style={{ float: "inherit" }}
                    >
                      <button
                        onClick={this.AddupdateDepartmentModel}
                        type="button"
                        className="btn-black mr-1 ems_manage_client_mng"
                        data-toggle="modal"
                      >
                        Add Department
                        <i className="menu-icon fa fa-plus"></i>
                      </button>
                    </div>
                  ) : null}

                  <button
                    type="button"
                    className="btn-black mr-1 ems_client_data_filter"
                    onClick={() => {
                      this.setState({
                        lstDepartment: this.state.lstDepartmentAll,
                        SelectedDepartment: -1,
                        FilterStatus: null,
                      });
                    }}
                  >
                    Clear
                  </button>
                  <button
                    onClick={this.DepartmentFilterModel}
                    type="button"
                    className="btn-black mr-1 ems_manage_client_mng"
                    data-toggle="modal"
                  >
                    Filters
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div className="row justify-content-center pt-2">
            <div className="col-md-6">
              <div className="card mt-3">
                <div className="card-body" style={{ paddingTop: "0px" }}>
                  <TableContainer
                    className="table-responsive"
                    component={"div"}
                  >
                    <Table className="Tabel">
                      <TableHead className="main_tbl_head">
                        <TableRow>
                          <TableCell className="text-center">
                            Department Name
                          </TableCell>
                          <TableCell className="text-center">Status</TableCell>
                        </TableRow>
                      </TableHead>

                      <TableBody>
                        {this.state.lstDepartment.length > 0 ? (
                          this.state.lstDepartment
                            .slice(
                              this.state.page * this.state.rowsPerPage,
                              this.state.page * this.state.rowsPerPage +
                                this.state.rowsPerPage
                            )
                            .map((item, key) => {
                              return (
                                <>
                                  {Cookies.get("Role") === "SuperAdmin" ? (
                                    <TableRow
                                      key={key}
                                      onClick={() => {
                                        this.setState({
                                          DepartmentHandle: {
                                            DepartmentName: item.DepartmentName,

                                            DepartmentIsActive:
                                              item.IsActive === true
                                                ? "Active"
                                                : "In-Active",
                                            ID: item.ID,
                                          },
                                          AddupdateDepartmentModel: true,
                                          Title: "Update Department",
                                          AddDepartButton: "Update",
                                        });
                                      }}
                                      style={{ cursor: "pointer" }}
                                    >
                                      <TableCell className="text-center">
                                        {item.DepartmentName}
                                      </TableCell>
                                      <TableCell className="text-center">
                                        {item.IsActive == true
                                          ? "Active"
                                          : "InActive"}
                                      </TableCell>
                                    </TableRow>
                                  ) : (
                                    <TableRow key={key}>
                                      <TableCell>
                                        {item.DepartmentName}
                                      </TableCell>
                                      <TableCell>
                                        {item.IsActive == true
                                          ? "Active"
                                          : "InActive"}
                                      </TableCell>
                                    </TableRow>
                                  )}
                                </>
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
                              No Record Available
                            </TableCell>
                          </TableRow>
                        )}
                      </TableBody>
                    </Table>
                  </TableContainer>
                  <TablePagination
                    component="div"
                    count={this.state.lstDepartment.length}
                    page={this.state.page}
                    onChangePage={this.handleChangePage}
                    rowsPerPage={this.state.rowsPerPage}
                    onChangeRowsPerPage={this.handleChangeRowsPerPage}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
        {this.state.AddupdateDepartmentModel === true ? (
          <AddUpdateDepartment
            Title={this.state.Title}
            show={this.state.AddupdateDepartmentModel}
            onHide={this.closeAddupdateDepartment}
            errorclass={this.state.error_class}
            HandelErrorRemove={this.HandelErrorRemove}
            DepartmentHandle={this.state.DepartmentHandle}
            AddDepartmentHandleChange={this.AddDepartmentHandleChange}
            AddDepartButton={this.state.AddDepartButton}
            AddDepartFun={this.validatePopUp}
            DeleteRecord={this.DeleteRecord}
          />
        ) : null}
        {this.state.DepartmentFilterstate === true ? (
          <DepartmentFilter
            Title={"Department Filter"}
            show={this.state.DepartmentFilterstate}
            onHide={() => {
              this.setState({ DepartmentFilterstate: false });
            }}
            DepartList={this.state.lstDepartment}
            changedepartmentState={(e) => {
              this.setState({
                SelectedDepartment: e.target.value,
              });
            }}
            ChangedStatus={(e) => {
              this.setState({
                FilterStatus: e.target.value,
              });
            }}
            FilterStatus={this.state.FilterStatus}
            SelectedDepartment={this.state.SelectedDepartment}
            onSubmit={this.SearchDepartmentGrid}
          />
        ) : null}
        {this.state.show == true ? (
          <PopUpMsgDepartment
            show={this.state.show}
            Title={this.state.Title}
            ShowMsgs={this.state.ShowMsgs}
            onHide={() => {
              this.setState({ show: false, Title: null, ShowMsgs: null });
            }}
          />
        ) : null}
      </div>
    );
  }
}

export default Department;
