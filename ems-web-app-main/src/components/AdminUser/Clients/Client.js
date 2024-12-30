import React, { Component } from "react";
import axios from "axios";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import AddClientPopup from "./AddClientPopup";
import Cookies from "js-cookie";
import Loader from "../../Loader/Loader";
import SureMsgPopUp from "./SureMsgPopUp";
import ClientwarningPopup from "./ClientwarningPopup";
import { encrypt, decrypt } from "react-crypt-gsm";
class Client extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showdropdown: false,
      SearchClient: null,
      Loading: true,
      AddClientShow: false,
      ConfrimMsgPopUp: false,
      AddClientTitle: null,
      AddClientButton: null,
      lstClient: [],
      lstClientAll: [],
      page: 0,
      Title: null,
      errorMsg: null,
      rowsPerPage: 10,
      ClientHandle: {
        ClientName: "",
        ClientEmail: "",
        ClientContact: "",
        ClientAddress: "",
        ClientWebsite: "",
        ClientFacebook: "",
        ClientTwitter: "",
        ClientInstagram: "",
        Status: "",
        Id: 0,
      },
      error_class: {
        ClientName: "",
        ClientEmail: "",
        ClientContact: "",
        ClientAddress: "",
        ClientWebsite: "",
        ClientFacebook: "",
        ClientTwitter: "",
        ClientInstagram: "",
        Status: "",
      },
      SearchClientvalue: null,
    };
  }
  CloseModalAfterAdd = () => {
    this.setState({ ConfrimMsgPopUp: false });
  };
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
  };

  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };

  showAddClientBox = () => {
    this.setState({
      ClientHandle: {
        ClientName: "",
        ClientEmail: "",
        ClientContact: "",
        ClientAddress: "",
        ClientWebsite: "",
        ClientFacebook: "",
        ClientTwitter: "",
        ClientInstagram: "",
        Status: "",
        Id: 0,
      },
      AddClientShow: true,
      AddClientTitle: "Add Client",
      AddClientButton: "Save",
    });
  };

  AddClientHandleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;

      name === "ClientContact"
        ? (value = value.replace(/\D/g, ""))
        : (value = target.value);

      this.setState({
        ClientHandle: {
          ...this.state.ClientHandle,
          [name]: value,
        },
      });
    } catch (ex) {
      alert(ex);
    }
  };

  validatePopUp = () => {
    var checkError = false;
    if (this.state.ClientHandle.ClientName === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          ClientName: "input_error",
        },
      }));
    }
    if (this.state.ClientHandle.ClientEmail === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          ClientEmail: "input_error",
        },
      }));
    }
    if (this.state.ClientHandle.ClientContact === "") {
      checkError = true;
      this.setState((prevState) => ({
        error_class: {
          ...prevState.error_class,
          ClientContact: "input_error",
        },
      }));
    }
    if (this.state.ClientHandle.ClientEmail !== "") {
      var checkTask = this.state.lstClient.filter((Email) =>
        Email.ClientEmail.toLowerCase().includes(
          this.state.ClientHandle?.ClientEmail.trim().toLowerCase()
        )
      );
      if (
        checkTask.length > 0 &&
        checkTask[0]?.ID !== this.state.ClientHandle?.Id
      ) {
        checkError = true;
        this.setState({ errorMsg: "Client Alert" });
        this.setState({
          errorMsg:
            `Email "` + this.state.ClientHandle?.ClientEmail + `" Already exit`,
        });
        this.setState({ ShowMsgPopUp: true });
      } else {
        if (checkTask.length > 1) {
          checkError = true;
          this.setState({ errorMsg: "Client Alert" });
          this.setState({
            errorMsg:
              `Email "` +
              this.state.ClientHandle?.ClientEmail +
              `" Already exit`,
          });
          this.setState({ ShowMsgPopUp: true });
        }
      }
    }
    if (!checkError) {
      this.AddClients();
    }
  };

  CloseModal = () => {
    this.setState({ ShowMsgPopUp: false });
  };
  CheckPopUp = () => {
    this.setState({ ShowMsgPopUp: false });
  };

  HandelErrorRemove = (name) => {
    if (name === "ClientName") {
      if (this.state.ClientHandle.ClientName === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            ClientName: "",
          },
        }));
      }
    }
    if (name === "ClientEmail") {
      if (this.state.ClientHandle.ClientEmail === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            ClientEmail: "",
          },
        }));
      }
    }
    if (name === "ClientContact") {
      if (this.state.ClientHandle.ClientContact === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            ClientContact: "",
          },
        }));
      }
    }
    if (name === "Status") {
      if (this.state.ClientHandle.Status === "") {
        this.setState((prevState) => ({
          error_class: {
            ...prevState.error_class,
            Status: "",
          },
        }));
      }
    }
  };

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
    this.LoadClients();
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
  LoadClients = () => {
    try {
      this.setState({ Loading: true, SearchClient: null });
      var Payload = {
        ID: 0,
        ClientName: "",
        ClientEmail: "",
        ContactNumber: "",
        Address: "",
        Website_URL: "",
        Facebooklink: "",
        Twitter: "",
        instagramlink: "",
        Active: 0,
      };

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Client/GetClientList`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: Payload,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstClient: res.data.Result,
          lstClientAll: res.data.Result,
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

  AddClients = () => {
    try {
      var AddClient = {
        ID: 0,
        ClientName: "",
        ClientEmail: "",
        ContactNumber: "",
        Address: "",
        Website_URL: "",
        Facebooklink: "",
        Twitter: "",
        instagramlink: "",
        Active: 0,
      };

      if (
        !/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(
          this.state.ClientHandle.ClientEmail
        )
      ) {
        this.setState({
          Title: "Email Address Invalid",
        });
        this.setState({
          errorMsg: `Please Enter Valid Email Address`,
        });
        this.setState({ ConfrimMsgPopUp: true });
        return false;
      }
      if (this.state.ClientHandle.Id === 0) {
        AddClient = {
          ID: 0,
          ClientName: this.state.ClientHandle.ClientName,
          ClientEmail: this.state.ClientHandle.ClientEmail,
          ContactNumber: this.state.ClientHandle.ClientContact,
          Address: this.state.ClientHandle.ClientAddress,
          Website_URL: this.state.ClientHandle.ClientWebsite,
          Facebooklink: this.state.ClientHandle.ClientFacebook,
          Twitter: this.state.ClientHandle.ClientTwitter,
          instagramlink: this.state.ClientHandle.ClientInstagram,
          Active: this.state.ClientHandle.Status === "Active" ? 1 : 0,
        };
      } else {
        AddClient = {
          ID: this.state.ClientHandle.Id,
          ClientName: this.state.ClientHandle.ClientName,
          ClientEmail: this.state.ClientHandle.ClientEmail,
          ContactNumber: this.state.ClientHandle.ClientContact,
          Address: this.state.ClientHandle.ClientAddress,
          Website_URL: this.state.ClientHandle.ClientWebsite,
          Facebooklink: this.state.ClientHandle.ClientFacebook,
          Twitter: this.state.ClientHandle.ClientTwitter,
          instagramlink: this.state.ClientHandle.ClientInstagram,
          Active: this.state.ClientHandle.Status === "Active" ? 1 : 0,
        };
      }

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Client/InsertUpdateClient`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: AddClient,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState({
            ClientHandle: {
              ClientName: "",
              ClientEmail: "",
              ClientContact: "",
              ClientAddress: "",
              ClientWebsite: "",
              ClientFacebook: "",
              ClientTwitter: "",
              ClientInstagram: "",
              Status: "",
              Id: 0,
            },
          });
          this.setState({ AddClientShow: false }, () => {
            this.LoadClients();
          });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };

  DeleteRecord = () => {
    try {
      const DelClient = {
        ID: this.state.ClientHandle.Id,
      };

      axios({
        method: "Post",
        url: `${process.env.REACT_APP_BASE_URL}Client/DeleteClient`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: DelClient,
      }).then((res) => {
        if (res?.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.StatusCode === 200) {
          this.setState({
            ClientHandle: {
              ClientName: "",
              ClientEmail: "",
              ClientContact: "",
              ClientAddress: "",
              ClientWebsite: "",
              ClientFacebook: "",
              ClientTwitter: "",
              ClientInstagram: "",
              Status: "",
              Id: 0,
            },
          });

          this.setState({ AddClientShow: false }, () => {
            this.LoadClients();
          });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  FilterUpdateClient = () => {
    if (this.state.SearchClientvalue === null) {
      this.setState({
        showdropdown: false,
      });
      var checkClient = this.state.lstClientAll.filter((SName) =>
        SName.ClientName.trim()
          .toLowerCase()
          .includes(this.state.SearchClientvalue.trim().toLowerCase())
      );
      this.setState({ lstClient: checkClient });
    } else if (this.state.SearchClientvalue != null) {
      var checkClient = this.state.lstClientAll.filter((SName) =>
        SName.ClientName.trim()
          .toLowerCase()
          .includes(this.state.SearchClientvalue.trim().toLowerCase())
      );
      this.setState({ lstClient: checkClient });
      //this.SearchMainTask();
    } else {
      this.setState({
        lstClient: this.state.lstClientAll,
        SearchClient: null,
      });
    }
  };
  FilterClient = (e) => {
    this.setState(
      {
        SearchClient: e.target.value,
        showdropdown: true,
        SearchClientvalue: e.target.value,
      },
      () => {
        if (this.state.SearchClient === "") {
          this.setState({
            showdropdown: false,
          });
          var checkClient = this.state.lstClientAll.filter((SName) =>
            SName.ClientName.trim()
              .toLowerCase()
              .includes(this.state.SearchClient.trim().toLowerCase())
          );
          this.setState({ lstClient: checkClient });
        } else if (this.state.SearchClient != null) {
          var checkClient = this.state.lstClientAll.filter((SName) =>
            SName.ClientName.trim()
              .toLowerCase()
              .includes(this.state.SearchClient.trim().toLowerCase())
          );
          this.setState({ lstClient: checkClient });
          //this.SearchMainTask();
        } else {
          this.setState({
            lstClient: this.state.lstClientAll,
            SearchClient: null,
          });
        }
      }
    );
  };
  _onSearchSelect = (e) => {
    var _ClientID = e.target.getAttribute("ddAttrClientID");
    var value = e.target.value;
    var _ClientName = e.target.getAttribute("ddAttrClientName");
    var checkClient = this.state.lstClientAll.filter((SName) =>
      SName.ClientName.trim()
        .toLowerCase()
        .includes(_ClientName.trim().toLowerCase())
    );
    this.setState({
      lstClient: checkClient,
      showdropdown: false,
      SearchClient: _ClientName,
      SearchClientvalue: _ClientName,
    });
  };
  render() {
    console.log("Client Rendering !");
    var SearchClientList = null;
    SearchClientList =
      this.state.lstClient !== null
        ? this.state.lstClient.map((item) => {
            return (
              <li
                onClick={this._onSearchSelect}
                ddAttrClientID={item.ID}
                ddAttrClientName={item.ClientName}
              >
                {item.ClientName}
              </li>
            );
          })
        : "";
    return (
      <div id="ems_client_view">
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="row rpt_mbl_header">
                <div
                  className="col-sm-4 text-left"
                  style={{ marginBottom: "10px" }}
                >
                  <h3 className="m_font_heading">Manage Clients</h3>
                </div>

                <div
                  className="col-sm-8 text-right"
                  id="ems_user_manage_clients"
                >
                  <div className="input_list transfer_list_search">
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
                    {this.state.showdropdown == true ? (
                      this.state.lstClient != null ? (
                        this.state.lstClient.length > 0 ? (
                          <div className="input_box">
                            <ul>{SearchClientList}</ul>
                          </div>
                        ) : null
                      ) : null
                    ) : null}
                  </div>
                  {Cookies.get("Role") === "SuperAdmin" ? (
                    <div
                      class="custom_select_latest_outer"
                      style={{ float: "inherit" }}
                    >
                      <button
                        onClick={this.showAddClientBox}
                        type="button"
                        className="btn-black mr-1 ems_manage_client_mng"
                        data-toggle="modal"
                        data-target="#AddSubTask"
                      >
                        Add Client
                        <i className="menu-icon fa fa-plus"></i>
                      </button>
                    </div>
                  ) : (
                    ""
                  )}
                  <button
                    onClick={() => {
                      this.setState(
                        {
                          SearchClientvalue: null,
                        },
                        () => {
                          this.LoadClients();
                        }
                      );
                    }}
                    type="button"
                    className="btn-black mr-1 ems_client_data_filter"
                    data-toggle="modal"
                    data-target="#AddSubTask"
                  >
                    Clear
                  </button>
                </div>
              </div>
            </div>
          </div>
          {this.state.Loading ? (
            <Loader />
          ) : (
            <div className="card mt-3">
              <div className="card-body" style={{ paddingTop: "0px" }}>
                <TableContainer className="table-responsive" component={"div"}>
                  <Table className="Tabel">
                    <TableHead className="main_tbl_head">
                      <TableRow>
                        <TableCell>Name</TableCell>
                        <TableCell>Email</TableCell>
                        <TableCell>Contact</TableCell>
                        <TableCell>Address</TableCell>
                        <TableCell>Website</TableCell>
                        <TableCell>Facebook</TableCell>
                        <TableCell>Twitter</TableCell>
                        <TableCell>Instagram</TableCell>
                        <TableCell>Active</TableCell>
                      </TableRow>
                    </TableHead>

                    <TableBody>
                      {this.state.lstClient.length > 0 ? (
                        this.state.lstClient
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
                                        ClientHandle: {
                                          ClientName: item.ClientName,
                                          ClientEmail: item.ClientEmail,
                                          ClientContact: item.ContactNumber,
                                          ClientAddress: item.Address,
                                          ClientWebsite: item.Website_URL,
                                          ClientFacebook: item.Facebooklink,
                                          ClientTwitter: item.Twitter,
                                          ClientInstagram: item.instagramlink,
                                          Status:
                                            item.Active === true
                                              ? "Active"
                                              : "In-Active",
                                          Id: item.ID,
                                        },
                                        AddClientShow: true,
                                        AddClientTitle: "Update Client",
                                        AddClientButton: "Update",
                                      });
                                    }}
                                    style={{ cursor: "pointer" }}
                                  >
                                    <TableCell>{item.ClientName}</TableCell>
                                    <TableCell>{item.ClientEmail}</TableCell>
                                    <TableCell>{item.ContactNumber}</TableCell>
                                    <TableCell>{item.Address}</TableCell>
                                    <TableCell>
                                      <span>{item.Website_URL}</span>
                                    </TableCell>
                                    <TableCell>{item.Facebooklink}</TableCell>
                                    <TableCell>{item.Twitter}</TableCell>
                                    <TableCell>{item.instagramlink}</TableCell>
                                    <TableCell>{String(item.Active)}</TableCell>
                                  </TableRow>
                                ) : (
                                  <TableRow>
                                    <TableCell>{item.ClientName}</TableCell>
                                    <TableCell>{item.ClientEmail}</TableCell>
                                    <TableCell>{item.ContactNumber}</TableCell>
                                    <TableCell>{item.Address}</TableCell>
                                    <TableCell>
                                      <span>{item.Website_URL}</span>
                                    </TableCell>
                                    <TableCell>{item.Facebooklink}</TableCell>
                                    <TableCell>{item.Twitter}</TableCell>
                                    <TableCell>{item.instagramlink}</TableCell>
                                    <TableCell>{String(item.Active)}</TableCell>
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
                  count={this.state.lstClient.length}
                  page={this.state.page}
                  onChangePage={this.handleChangePage}
                  rowsPerPage={this.state.rowsPerPage}
                  onChangeRowsPerPage={this.handleChangeRowsPerPage}
                />
              </div>
            </div>
          )}
        </div>

        <AddClientPopup
          AddClientTitle={this.state.AddClientTitle}
          AddClientButton={this.state.AddClientButton}
          AddClientShow={this.state.AddClientShow}
          AddClientHandle={this.state.ClientHandle}
          AddClientHandleChange={this.AddClientHandleChange}
          // AddClientFun={this.AddClients}
          closeAddClientBox={() => {
            this.setState({ AddClientShow: false });
          }}
          errorclass={this.state.error_class}
          AddClientFun={this.validatePopUp}
          DeleteRecord={this.DeleteRecord}
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
        <ClientwarningPopup
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.ConfrimMsgPopUp}
          onHide={this.CloseModalAfterAdd}
        />
      </div>
    );
  }
}

export default Client;
