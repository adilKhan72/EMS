import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import ProjectImageCrop from "../ImagePopUp/ProjectImageCrop";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import "./AddProject.css";
import moment from "moment";
import axios from "axios";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "react-crypt-gsm";
class AddProjectPopUp extends Component {
  constructor(props) {
    super(props);
    this.state = {
      page: 0,
      rowsPerPage: 3,
      ShowBudgetMenu: false,
      lstClient: null,
    };
    this.MonthlyBudgetDD = React.createRef();
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
    this.LoadClients();
    document.addEventListener("mousedown", this.handleClickOutside);
  }
  componentWillUnmount() {
    document.removeEventListener("mousedown", this.handleClickOutside);
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
    this.setState({ rowsPerPage: parseInt(event.target.value, 3) });
    this.setState({ page: 0 });
  };
  closeAddProject = () => {
    this.setState({ ShowBudgetMenu: false });
    this.props.onHide();
  };
  handleClickOutside = (event) => {
    if (this.MonthlyBudgetDD.current != null) {
      if (
        this.MonthlyBudgetDD &&
        !this.MonthlyBudgetDD.current.contains(event.target)
      ) {
        this.setState({ ShowBudgetMenu: false });
      }
    }
  };
  LoadClients = () => {
    try {
      this.setState({ Loading: true });
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
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        this.setState({
          lstClient: res.data.Result,
        });
        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  render() {
    var TempClientList =
      this.state.lstClient == null
        ? ""
        : this.state.lstClient?.map((item) => {
            return (
              <option value={item.ID} name={item.ClientName}>
                {item.ClientName}
              </option>
            );
          });
    var month = moment(this.props.CurrentBudgetYear).format("MMMM"); //get the current month to show in monthly budget
    return (
      <Modal
        show={this.props.AddProjectShow}
        onHide={this.closeAddProject}
        backdrop={"static"}
        keyboard={"false"}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div
          class="modal-content"
          style={{ boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "0.2rem" }}
          ></Modal.Header>
          <Modal.Body>
            <>
              <div class="modal-body material_style">
                <form action="" method="post">
                  <div class="row align-items-center">
                    <div class="col-md-2">
                      <div class="form-group datepicker_mbl">
                        <div class="profile-placeholder">
                          <div class="circle">
                            <img
                              class="profile-pic"
                              src={localStorage.getItem("ProjectImg")}
                              /* src="images/profile-avator.png" */
                              alt=""
                            />
                          </div>
                          <div
                            class="add-avator"
                            onClick={() => {
                              this.props.imageOnShow();
                            }}
                          >
                            <i class="fa fa-plus upload-button"></i>
                          </div>
                        </div>
                      </div>
                    </div>
                    <div
                      //className="form-group col-md-6 mb-1"
                      className="col-md-3"
                    >
                      <div
                        //className="form-group  mb-2 styled-select"
                        className={
                          "form-group  mb-2 styled-select " +
                          this.props.errorStyle.Client
                        }
                      >
                        <select
                          //onFocus={this.props.ErrorStyleRemovedAddProject}
                          onFocus={() => {
                            this.props.HandelErrorRemove("Client");
                          }}
                          //style={this.props.FieldErrorStyle.ProjectType}
                          name="Client"
                          id="select"
                          class="form-control"
                          onChange={this.props.HandleAddProject}
                          required
                        >
                          <option value="" disabled selected hidden>
                            {/* --Select-- */}
                          </option>
                          {TempClientList}
                        </select>
                        <label>Client</label>
                      </div>
                    </div>
                    <div
                      //className="form-group col-md-6 mb-1"
                      className={
                        "form-group col-md-7 mb-1 " +
                        this.props.errorStyle.ProjectName
                      }
                    >
                      <input
                        //onFocus={this.props.ErrorStyleRemovedAddProject}
                        onFocus={() => {
                          this.props.HandelErrorRemove("ProjectName");
                        }}
                        //style={this.props.FieldErrorStyle.ProjectName}
                        name="ProjectName"
                        type="text"
                        className="form-control modal_inputs"
                        onChange={this.props.HandleAddProject}
                        required
                        autoComplete="off"
                      />
                      <label>Project Name</label>
                    </div>
                  </div>
                  <div class="row">
                    <div className="col-md-4">
                      <div
                        className={
                          this.props.ShowStartDate == null
                            ? "form-group  mb-2 datepicker_mbl date_value_added " +
                              this.props.errorStyle.StartDate
                            : "form-group mb-2 datepicker_mbl " +
                              this.props.errorStyle.StartDate
                        }
                      >
                        <div className="customDatePickerWidth">
                          <label className="datepicker_label">Start Date</label>
                          <DatePicker
                            /*  style={this.props.FieldErrorStyle.StartDate} */
                            name="StartDate"
                            dateFormat="dd/MM/yyyy"
                            //placeholderText={"DD/MM/YYYY"}
                            selected={this.props.ShowStartDate}
                            shouldCloseOnSelect={true}
                            className="form-control"
                            onChange={this.props.HandleStartDateChange}
                            required
                            autoComplete="off"
                            onFocus={() => {
                              this.props.HandelErrorRemove("StartDate");
                            }}
                          />
                        </div>
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div
                        className={
                          this.props.ShowEndDate == null
                            ? "form-group  mb-2 datepicker_mbl date_value_added " +
                              this.props.errorStyle.EndDate
                            : "form-group mb-2 datepicker_mbl " +
                              this.props.errorStyle.EndDate
                        }
                      >
                        <div className="customDatePickerWidth">
                          <label className="datepicker_label">End Date</label>
                          <div className="modal_inputs">
                            <DatePicker
                              /*  style={this.props.FieldErrorStyle.EndDate} */
                              name="EndDate"
                              dateFormat="dd/MM/yyyy"
                              //placeholderText={"DD/MM/YYYY"}
                              selected={this.props.ShowEndDate}
                              shouldCloseOnSelect={true}
                              className="form-control"
                              onChange={this.props.HandleEndDateChange}
                              required
                              autoComplete="off"
                              onFocus={() => {
                                this.props.HandelErrorRemove("EndDate");
                              }}
                            />
                          </div>
                        </div>
                      </div>
                    </div>
                    {/* <div className="col-md-4">
                      <div
                        //className="form-group  mb-2"
                        className={
                          "form-group  mb-2 " + this.props.errorStyle.TaskOwner
                        }
                      >
                        <input
                          //onFocus={this.props.ErrorStyleRemovedAddProject}
                          onFocus={() => {
                            this.props.HandelErrorRemove("TaskOwner");
                          }}
                          //style={this.props.FieldErrorStyle.TaskOwner}
                          name="TaskOwner"
                          type="text"
                          //placeholder="Task Owner"
                          className="form-control"
                          onChange={this.props.HandleAddProject}
                          required
                          autoComplete="off"
                        />
                        <label>Project Owner</label>
                      </div>
                    </div> */}
                    <div className="col-md-4">
                      <div
                        //className="form-group  mb-2 styled-select"
                        className={
                          "form-group  mb-2 styled-select " +
                          this.props.errorStyle.ProjectType
                        }
                      >
                        <select
                          //onFocus={this.props.ErrorStyleRemovedAddProject}
                          onFocus={() => {
                            this.props.HandelErrorRemove("ProjectType");
                          }}
                          //style={this.props.FieldErrorStyle.ProjectType}
                          name="ProjectType"
                          id="select"
                          class="form-control"
                          onChange={this.props.HandleAddProject}
                          required
                        >
                          <option value="" disabled selected hidden>
                            {/* --Select-- */}
                          </option>
                          <option value="Digital Marketing">
                            Digital Marketing
                          </option>
                          <option value="Software Development">
                            Software Development
                          </option>
                        </select>
                        <label>Project Type</label>
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div
                        className="form-group  mb-2"
                        ref={this.MonthlyBudgetDD}
                        /*  className={
                          "form-group  mb-2 " +
                          this.state.errorStyle.ProjectType
                        } */

                        style={{ position: "relative" }}
                      >
                        <input
                          name="MonthlyBudget"
                          type="text"
                          //placeholder="Monthly Budget"
                          className="form-control "
                          value={this.props.MonthBudgetState[month]}
                          onChange={this.props.HandleAddProject}
                          onClick={() => {
                            if (this.state.ShowBudgetMenu == false) {
                              this.setState({ ShowBudgetMenu: true });
                            } else {
                              this.setState({ ShowBudgetMenu: false });
                            }
                          }}
                          autoComplete="off"
                        />
                        <label>Monthly Budget</label>
                        {this.state.ShowBudgetMenu ? (
                          <div
                            className="dropdown-menu show "
                            style={{
                              borderRadius: "10px",
                              top: "45px",
                              position: "absolute",
                            }}
                          >
                            {this.props.ShowStartDate != "" &&
                            this.props.ShowEndDate != "" ? (
                              <div className="HideTablePage">
                                <TableContainer component={"div"}>
                                  <Table id="bootstrap-data-table">
                                    <TableHead>
                                      <TableRow>
                                        <TableCell
                                          colSpan="2"
                                          style={{
                                            display: "flex",
                                            padding: "0",
                                            borderBottom: "0",
                                          }}
                                        >
                                          <p
                                            style={{
                                              padding: "5px 0 0 15px",
                                              fontWeight: "bold",
                                              color: "rgba(0, 0, 0, 0.87)",
                                            }}
                                          >
                                            Years:
                                            {moment(
                                              this.props.ShowStartDate
                                            ).format("YYYY") ==
                                            moment(
                                              this.props.ShowEndDate
                                            ).format("YYYY")
                                              ? moment(
                                                  this.props.CurrentBudgetYear
                                                ).format("YYYY")
                                              : moment(
                                                  this.props.ShowStartDate
                                                ).format("YYYY") +
                                                "-" +
                                                moment(
                                                  this.props.ShowEndDate
                                                ).format("YYYY")}
                                          </p>

                                          {/* <i
                                            style={{
                                              padding: "10px 0 0 15px",
                                              fontWeight: "bold",
                                              color: "rgba(0, 0, 0, 0.87)",
                                            }}
                                            onClick={() => {
                                              this.props.DecrementYear();
                                            }}
                                            class="fa fa-angle-left"
                                            aria-hidden="true"
                                          ></i>
                                          <i
                                            style={{
                                              padding: "10px 0 0 15px",
                                              fontWeight: "bold",
                                              color: "rgba(0, 0, 0, 0.87)",
                                            }}
                                            onClick={this.props.IncrementYear}
                                            class="fa fa-angle-right"
                                            aria-hidden="true"
                                          ></i> */}
                                        </TableCell>
                                      </TableRow>
                                      <TableRow>
                                        <TableCell
                                          style={{
                                            paddingRight: "80px",
                                            fontWeight: "bold",
                                          }}
                                        >
                                          Months:
                                        </TableCell>
                                        <TableCell
                                          style={{
                                            fontWeight: "bold",
                                          }}
                                        >
                                          Budget:
                                        </TableCell>
                                      </TableRow>
                                    </TableHead>
                                    <TableBody>
                                      {this.props.MonthArray.length > 0
                                        ? this.props.MonthArray.slice(
                                            this.state.page *
                                              this.state.rowsPerPage,
                                            this.state.page *
                                              this.state.rowsPerPage +
                                              this.state.rowsPerPage
                                          ).map((item) => {
                                            return (
                                              <TableRow>
                                                <TableCell>{item}</TableCell>
                                                <TableCell>
                                                  <input
                                                    type="number"
                                                    name={item}
                                                    value={
                                                      this.props
                                                        .MonthBudgetState[item]
                                                    }
                                                    onChange={
                                                      this.props
                                                        .HandleMonthBudgetProject
                                                    }
                                                    className="form-control col-md-7"
                                                  />
                                                </TableCell>
                                              </TableRow>
                                            );
                                          })
                                        : null}
                                    </TableBody>
                                  </Table>

                                  <div
                                    className="w-100 text-left col-sm-12"
                                    style={{ display: "flex" }}
                                  >
                                    <div
                                      style={{
                                        position: "relative",
                                        left: "-70px",
                                      }}
                                      className="col-8"
                                    ></div>
                                  </div>
                                </TableContainer>
                                <TablePagination
                                  rowsPerPageOptions={[]}
                                  component="div"
                                  count={this.props.MonthArray.length}
                                  page={this.state.page}
                                  onChangePage={this.handleChangePage}
                                  rowsPerPage={this.state.rowsPerPage}
                                  onChangeRowsPerPage={
                                    this.handleChangeRowsPerPage
                                  }
                                />
                              </div>
                            ) : (
                              <p
                                style={{
                                  margin: "0",
                                  fontSize: "12px",
                                  padding: "0 5px",
                                  fontWeight: "600",
                                  color: "#000",
                                }}
                              >
                                Please Select Date First
                              </p>
                            )}
                          </div>
                        ) : null}
                      </div>
                    </div>
                    <div className="col-md-4">
                      <div
                        className={
                          "form-group  mb-2 styled-select " +
                          this.props.errorStyle.Status
                        }
                      >
                        <select
                          onChange={this.props.HandleAddProject}
                          className="form-control"
                          name="Status"
                          onFocus={() => {
                            this.props.HandelErrorRemove("Status");
                          }}
                          value={this.props.AddStatus}
                          required
                        >
                          <option></option>
                          <option value="Active" name="Status">
                            Active
                          </option>
                          <option value="In-Active" name="Status">
                            In-Active
                          </option>
                        </select>

                        <label>Status</label>
                      </div>
                    </div>

                    <div className="col-md-8">
                      <div
                        className={
                          "form-group  mb-2 textarea_mbl " +
                          this.props.ProjectDescription
                          // this.props.errorStyle.ProjectDescription
                        }
                      >
                        <textarea
                          //onFocus={this.props.ErrorStyleRemovedAddProject}
                          onFocus={() => {
                            this.props.HandelErrorRemove("ProjectDescription");
                          }}
                          // style={this.props.FieldErrorStyle.ProjectDescription}
                          name="ProjectDescription"
                          className="ui-widget form-control txtArea text_boxshadow"
                          //placeholder=" Project Description"
                          rows="3"
                          cols="50"
                          onChange={this.props.HandleAddProject}
                          required
                        ></textarea>
                        <label className="modal_label">
                          Project Description
                        </label>
                        {/* <div
                          className=" col-md-12 textarea_mbl"
                        >
                          <div className="col-md-10 ">
                            
                            
                          </div>
                        </div> */}
                      </div>
                    </div>
                  </div>
                </form>
              </div>
            </>
          </Modal.Body>
          <Modal.Footer style={{ border: "none", padding: "0.2rem" }}>
            <div className="w-100 text-right py-3 px-4">
              <Button
                className="btn-black"
                onClick={this.props.CheckValidateAddProject}
              >
                Save
              </Button>
            </div>
          </Modal.Footer>

          <ProjectImageCrop
            Title={"Upload Photo"}
            show={this.props.ShowImageModal}
            onHide={() => {
              this.props.imageOnHide();
            }}
          />
        </div>
      </Modal>
    );
  }
}

export default AddProjectPopUp;
