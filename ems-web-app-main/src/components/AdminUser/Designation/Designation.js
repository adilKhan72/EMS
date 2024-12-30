import React, { Component } from "react";
import axios from "axios";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import AddDesignationPopup from "./AddDesignationPopup";
import SureMsgPopUp from "./SureMsgPopUp";
import Cookies from "js-cookie";
import ConfirmPopUpModal from "./ConfirmPopUpModal";
import Loader from "../../Loader/Loader";
import { encrypt, decrypt } from "react-crypt-gsm";
class Designation extends Component {
  constructor(props) {
    super(props);
    this.state = {
      AddDesignationShow: false,
      AddDesignationTitle: null,
      AddDesignationButton: null,
      FilterDesignationPopup: false,
      ConfirmDelete: true,
      ShowMsgPopUp: false,
      ConfrimMsgPopUp: false,
      lstDesignation: [],
      page: 0,
      rowsPerPage: 10,
      DesignationHandle: {
        DesignationName: "",
        Status: "",
        Id: 0,
      },
      FilterDesignation: null,
      error_class: {
        DesignationName: "",
        Status: "",
      },
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
    this.LoadDesignations();
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

  LoadDesignations = () => {
    try {
      this.setState({ Loading: true });
      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Designation/GetDesignation`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({ lstDesignation: res.data.Result });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  AddDesignationHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        DesignationHandle: {
          ...this.state.DesignationHandle,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };
  AddDesignations = () => {
    try {
      var AddDesignation = {
        Id: 0,
        DesignationName: null,
        Active: 0,
        Case: null,
      };

      if (this.state.DesignationHandle.Id === 0) {
        AddDesignation = {
          Id: 0,
          DesignationName: this.state.DesignationHandle.DesignationName,
          Active: this.state.DesignationHandle.Status === "Active" ? 1 : 0,
          Case: "add",
        };
      } else {
        AddDesignation = {
          Id: this.state.DesignationHandle.Id,
          DesignationName: this.state.DesignationHandle.DesignationName,
          Active: this.state.DesignationHandle.Status === "Active" ? 1 : 0,
          Case: null,
        };
      }

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Designation/InsertDesignation`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddDesignation,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState({
            DesignationHandle: {
              DesignationName: "",
              Status: "",
              Id: 0,
            },
          });
          this.setState({ AddDesignationShow: false }, () => {
            this.LoadDesignations();
          });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  closeDesignation = () => {
    this.setState({ error_class: {} });
    this.setState({ AddDesignationShow: false });
  };
  CloseModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CloseDeleteRecord = () => {
    this.setState({ ShowMsgPopUp: false });
  };

  CheckPopUp = () => {
    this.DeleteRecord();
  };

  ConfrimModal = () => {
    this.setState({ Title: "Delete Record" });
    this.setState({
      errorMsg:
        "Are you sure you want to Delete " +
        this.state.DesignationHandle?.DesignationName +
        " ?",
    });
    this.setState({ ShowMsgPopUp: true });
  };

  DeleteRecord = () => {
    try {
      const DelDesig = {
        Id: this.state.DesignationHandle.Id,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}Designation/DeleteDesignation`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DelDesig,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState({
            DesignationHandle: {
              DesignationName: "",
              Status: "",
              Id: 0,
            },
          });

          this.setState({ ShowMsgPopUp: false });
          this.setState({ AddDesignationShow: false }, () => {
            this.LoadDesignations();
          });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  validatePopUp = () => {
    var checkError = false;
    if (this.state.DesignationHandle.DesignationName === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          DesignationName: "input_error",
        },
      }));
    }

    if (this.state.DesignationHandle.DesignationName === "") {
    } else {
      var checkTask = this.state.lstDesignation.filter((Name) =>
        Name.DesignationName.toLowerCase().includes(
          this.state.DesignationHandle?.DesignationName.trim().toLowerCase()
        )
      );
      if (
        checkTask.length > 0 &&
        checkTask[0]?.Id !== this.state.DesignationHandle?.Id
      ) {
        checkError = true;
        this.setState({ Titleok: "Designation Alert" });
        this.setState({
          errorokMsg:
            `Designation Name "` +
            this.state.DesignationHandle?.DesignationName +
            `" Already exit`,
        });
        this.setState({ ConfrimMsgokPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ Titleok: "Designation Alert" });
          this.setState({
            errorokMsg:
              `Designation Name "` +
              this.state.DesignationHandle?.DesignationName +
              `" Already exit`,
          });
          this.setState({ ConfrimMsgokPopUp: true });
        }
      }
    }
    if (!checkError) {
      this.AddDesignations();
    }
  };
  CloseokModal = () => {
    this.setState({ ConfrimMsgokPopUp: false });
  };
  CheckokConfirm = () => {
    this.setState({ ConfrimMsgokPopUp: false });
  };
  HandelErrorRemove = (name) => {
    if (name === "DesignationName") {
      if (this.state.DesignationHandle.DesignationName === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            DesignationName: "",
          },
        }));
      }
    }
    if (name === "Status") {
      if (this.state.DesignationHandle.Status === "") {
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
                  <h3 className="m_font_heading ems_design">
                    Manage Designations
                  </h3>
                </div>

                <div className="col-sm-8 text-right" id="ems_user_designations">
                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <button
                      onClick={() => {
                        this.setState({
                          DesignationHandle: {
                            DesignationName: "",
                            Status: "",
                            Id: 0,
                          },
                          AddDesignationShow: true,
                          AddDesignationTitle: "Add Designations",
                          AddDesignationButton: "Save",
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
                      Add Designation
                      <i className="menu-icon fa fa-plus"></i>
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
            <div className="row justify-content-center pt-2">
              <div className="col-md-6">
                <div className="card" style={{ margintop: "20px" }}>
                  <div className="card-body" style={{ paddingTop: "0px" }}>
                    <TableContainer
                      className="table-responsive"
                      component={"div"}
                    >
                      <Table className="Tabel">
                        <TableHead className="main_tbl_head">
                          <TableRow>
                            <TableCell
                              style={{ width: "50%", textAlign: "center" }}
                            >
                              Designation Name
                            </TableCell>
                            <TableCell
                              style={{ width: "50%", textAlign: "center" }}
                            >
                              Active
                            </TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {this.state.lstDesignation.length > 0 ? (
                            this.state.lstDesignation
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
                                        style={{
                                          cursor: "pointer",
                                          width: "50%",
                                        }}
                                        onClick={() => {
                                          this.setState({
                                            DesignationHandle: {
                                              DesignationName:
                                                item.DesignationName,
                                              Status:
                                                item.Active === true
                                                  ? "Active"
                                                  : "In-Active",
                                              Id: item.Id,
                                            },
                                            AddDesignationShow: true,
                                            AddDesignationTitle:
                                              "Update Designation",
                                            AddDesignationButton: "Update",
                                          });
                                        }}
                                      >
                                        <TableCell
                                          component="th"
                                          scope="row"
                                          style={{ textAlign: "center" }}
                                        >
                                          {item.DesignationName}
                                        </TableCell>
                                        <TableCell
                                          style={{ textAlign: "center" }}
                                        >
                                          {String(item.Active)}
                                        </TableCell>
                                      </TableRow>
                                    ) : (
                                      <TableRow>
                                        <TableCell
                                          component="th"
                                          scope="row"
                                          style={{ textAlign: "center" }}
                                        >
                                          {item.DesignationName}
                                        </TableCell>
                                        <TableCell
                                          style={{ textAlign: "center" }}
                                        >
                                          {String(item.Active)}
                                        </TableCell>
                                      </TableRow>
                                    )}
                                  </>
                                );
                              })
                          ) : (
                            <TableRow>
                              <TableCell component="th" scope="row">
                                NO Designation
                              </TableCell>
                            </TableRow>
                          )}
                        </TableBody>
                      </Table>
                    </TableContainer>
                    <TablePagination
                      component="div"
                      count={this.state.lstDesignation.length}
                      page={this.state.page}
                      onChangePage={this.handleChangePage}
                      rowsPerPage={this.state.rowsPerPage}
                      onChangeRowsPerPage={this.handleChangeRowsPerPage}
                    />
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
        <AddDesignationPopup
          AddDesignationTitle={this.state.AddDesignationTitle}
          AddDesignationButton={this.state.AddDesignationButton}
          AddDesignationShow={this.state.AddDesignationShow}
          AddDesignationHandle={this.state.DesignationHandle}
          AddDesignationHandleChange={this.AddDesignationHandleChange}
          // AddDesignationFun={this.AddDesignations}
          closeDesignation={this.closeDesignation}
          errorclass={this.state.error_class}
          AddDesignationFun={this.validatePopUp}
          DeleteRecord={this.ConfrimModal}
          HandelErrorRemove={this.HandelErrorRemove}
        />
        <SureMsgPopUp
          Title={this.state.Title}
          ShowMsgs={this.state.errorMsg}
          show={this.state.ShowMsgPopUp}
          onConfrim={this.CheckPopUp}
          onHide={this.CloseModal}
          DeleteStatus={this.ChangeStatus}
        />
        <ConfirmPopUpModal
          Title={this.state.Titleok}
          ShowMsgs={this.state.errorokMsg}
          show={this.state.ConfrimMsgokPopUp}
          onConfrim={this.CheckokConfirm}
          onHide={this.CloseokModal}
        />
      </div>
    );
  }
}
export default Designation;
