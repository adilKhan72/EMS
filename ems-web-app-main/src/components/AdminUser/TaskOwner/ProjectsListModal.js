import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
import moment from "moment";
import DatePicker from "react-datepicker";
import axios from "axios";
import "react-datepicker/dist/react-datepicker.css";
import { encrypt, decrypt } from "react-crypt-gsm";
import Cookies from "js-cookie";

class ProjectsListModal extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Email: false,
      Password: false,
      onError: {
        Email: false,
        Password: false,
      },
      Email: "",
      Password: "",
      EmailCheck: false,
      PasswordCheck: false,
      ProjectList: [],
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
    this.LoadUserProject();
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
  LoadUserProject = () => {
    try {
      const UserIdObj = {
        UserID: this.props.UserIdForProjects,
      };
      axios({
        method: "post",
        url:
          `${process.env.REACT_APP_BASE_URL}ResourceMapping/GetProjectWithPercentage?TaskOWnerID=` +
          this.props.UserIdForProjects,
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
    return (
      <Modal
        show={this.props.UserProjectsShow}
        onHide={() => this.props.UserProjectsHide("hide")}
        backdrop={"static"}
        keyboard={"false"}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div className="modal-content">
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "1rem 0.2rem" }}
          >
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              {this.props.ModalTitle}
            </h5>
            {/* <p>
              {this.props.UserIdForProjects}
            </p> */}

            {/* <span class="clear_btn">Clear Search</span> */}
          </Modal.Header>
          <Modal.Body
            className="ems_project_list"
            style={{ marginTop: "-30px" }}
          >
            <div className="modal-body material_style p-0">
              <div className="row">
                <div className="col-12 p-0">
                  <div
                    className="card"
                    style={{
                      maxHeight: "400px",
                      boxShadow: "none",
                      border: "0px",
                    }}
                  >
                    <div
                      className="card-body"
                      style={{ maxHeight: "400px", padding: "0rem 1rem" }}
                    >
                      <TableContainer
                        className="table-responsive"
                        component={"div"}
                      >
                        <Table className="table">
                          <TableHead>
                            <TableRow>
                              <TableCell
                                className="text-left"
                                style={{ width: "50%" }}
                              >
                                Project Name
                              </TableCell>
                              <TableCell
                                className="text-center"
                                style={{ width: "30%" }}
                              >
                                Total Hours
                              </TableCell>
                              <TableCell
                                className="text-right"
                                style={{ width: "20%" }}
                              >
                                Status
                              </TableCell>
                            </TableRow>
                          </TableHead>
                          <TableBody>
                            {this.state.ProjectList.length > 0 ? (
                              this.state.ProjectList.map((item, index) => {
                                return (
                                  <TableRow>
                                    <TableCell className="text-left">
                                      {" "}
                                      {item.ProjectNames}
                                    </TableCell>
                                    <TableCell className="text-center">
                                      {" "}
                                      <div
                                        className="progress"
                                        style={{
                                          position: "relative",
                                          boxShadow: " 3px 4px 10px #888888",
                                        }}
                                      >
                                        <span
                                          style={{
                                            position: "absolute",
                                            zIndex: "1",
                                            left: "40%",
                                            transform: "translateY(-1px)",
                                            // transform: "translatex(-50%)",
                                            color: "black",
                                          }}
                                        >
                                          {item.percentage} hrs
                                        </span>
                                        <div
                                          className={"progress-bar bg-gradient"}
                                          role="progressbar"
                                          style={{
                                            width: `${item.percentage}%`,
                                          }}
                                          aria-valuenow={`${item.percentage}`}
                                          aria-valuemin="0"
                                          aria-valuemax="100"
                                        ></div>
                                      </div>
                                    </TableCell>
                                    <TableCell className="text-right">
                                      {" "}
                                      {item.IsActive == 1
                                        ? "Active"
                                        : "In-Active"}
                                    </TableCell>
                                  </TableRow>
                                );
                              })
                            ) : (
                              <TableRow
                                className="text-center"
                                style={{ width: "100%" }}
                              >
                                NO USER
                              </TableRow>
                            )}
                          </TableBody>
                        </Table>
                        {/*    <TablePagination
                      component="div"
                      count={this.state.lstUser.length}
                      page={this.state.page}
                      onChangePage={this.handleChangePage}
                      rowsPerPage={this.state.rowsPerPage}
                      onChangeRowsPerPage={this.handleChangeRowsPerPage}
                    /> */}
                      </TableContainer>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </Modal.Body>
        </div>
      </Modal>
    );
  }
}

export default ProjectsListModal;
