import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
import DatePicker from "react-datepicker";
import axios from "axios";
import "react-datepicker/dist/react-datepicker.css";

class AddTaskOwnersPopup extends Component {
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
    };
  }

  /*  handleValidation = (e) => {
    //
    if (this.state.Email == "") {
      this.setState({
        EmailCheck: true,
      });
    } else {
      this.setState({
        EmailCheck: false,
      });
    }

    if (this.state.Password == "") {
      this.setState({
        PasswordCheck: true,
      });
    } else {
      this.setState({
        PasswordCheck: false,
      });
    }
  }; */
  /*   handleChange = (e) => {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;

      this.setState(
        {
          [name]: value,
        },
        () => {
          this.handleValidation();
        }
      );
    } catch (ex) {
      alert(ex);
    }
  }; */
  render() {
    var DesignationNameList =
      this.props.DesignationRecord == null
        ? ""
        : this.props.DesignationRecord.map((item) => {
            return (
              <option value={item.DepartmentName}>
                {item.DesignationName}
              </option>
            );
          });
    var DepartmentList =
      this.props.DepartmentList == null
        ? ""
        : this.props.DepartmentList.map((item) => {
            return <option value={item.ID}>{item.DepartmentName}</option>;
          });
    return (
      <Modal
        show={this.props.AddTaskOwnersShow}
        onHide={this.props.closeAddTaskOwners}
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
            {/* <span class="clear_btn">Clear Search</span> */}
          </Modal.Header>
          <Modal.Body className="ems_update_users">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.UserName
                      }
                      type="text"
                      name="UserName"
                      value={this.props.TaskOwnerHandle.UserName}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("UserName");
                      }}
                      required
                      autoComplete="off"
                    />
                    <label>User Name</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      //className="form-control"
                      className={"form-control " + this.props.errorclass.Name}
                      type="text"
                      name="Name"
                      value={this.props.TaskOwnerHandle.Name}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("Name");
                      }}
                      required
                      autoComplete="off"
                    />
                    <label>Full Name</label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.Department
                      }
                      required
                      autoComplete="off"
                      name="Department"
                      value={this.props.TaskOwnerHandle.Department}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("Department");
                      }}
                    >
                      <option></option>
                      {DepartmentList}
                    </select>
                    <label>Department</label>
                    {/* <div className="modal_inputs  styled-select" >
                       
                    </div> */}
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.Designation
                      }
                      required
                      autoComplete="off"
                      name="Designation"
                      value={this.props.TaskOwnerHandle.Designation}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("Designation");
                      }}
                    >
                      <option></option>
                      {DesignationNameList}
                      {/* <option></option>
                       */}
                    </select>
                    <label>Designation</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      //className="form-control"
                      className={"form-control " + this.props.errorclass.Email}
                      type="text"
                      name="Email"
                      value={this.props.TaskOwnerHandle.Email}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("Email");
                      }}
                      required
                      autoComplete="off"
                    />
                    <label>Email Address</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={
                        "form-control " + this.props.errorclass.AccountType
                      }
                      required
                      autoComplete="off"
                      name="AccountType"
                      value={this.props.TaskOwnerHandle.AccountType}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("AccountType");
                      }}
                    >
                      <option></option>
                      <option>Staff</option>
                      <option>Admin</option>
                      <option>SuperAdmin</option>
                    </select>
                    <label>Account Type</label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      //className="form-control"
                      className={"form-control " + this.props.errorclass.Status}
                      required
                      autoComplete="off"
                      name="Status"
                      value={this.props.TaskOwnerHandle.Status}
                      onChange={this.props.TaskOwnerHandleChange}
                      onFocus={() => {
                        this.props.HandelErrorRemove("Status");
                      }}
                    >
                      {this.props.TaskOwnerHandle.Status != "" ? (
                        this.props.TaskOwnerHandle.Status == true ? (
                          <option value={1} selected hidden>
                            Active
                          </option>
                        ) : (
                          <option value={0} selected hidden>
                            In-Active
                          </option>
                        )
                      ) : (
                        <option value={0} selected hidden>
                          In-Active
                        </option>
                      )}
                      <option value={1}>Active</option>
                      <option value={0}>In-Active</option>
                    </select>
                    <label>Status</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div
                    className={
                      "form-group datepicker_mbl " +
                      this.props.errorclass.JoiningDate
                    }
                  >
                    {this.props.ShowJoiningDate == false ? (
                      <div
                        className="customDatePickerWidth custom_top"
                        id="ems_task_owner_calendar"
                      >
                        <DatePicker
                          type="date"
                          dateFormat="dd/MM/yyyy"
                          //placeholderText={"DD/MM/YYYY"}
                          selected={this.props.ShowJoiningDate}
                          shouldCloseOnSelect={true}
                          className="form-control"
                          /* className={
                            "form-control " + this.props.errorclass.JoiningDate
                          } */
                          onChange={this.props.JoinDateHandler}
                          onFocus={() => {
                            this.props.HandelErrorRemove("Date");
                          }}
                        />
                        <label>Joining Date</label>
                      </div>
                    ) : (
                      <div
                        className="customDatePickerWidth"
                        id="ems_task_owner_calendar"
                      >
                        <DatePicker
                          dateFormat="dd/MM/yyyy"
                          placeholderText={this.props.ShowJoiningDate}
                          //selected={this.props.ShowJoiningDate}
                          shouldCloseOnSelect={true}
                          className="form-control"
                          onChange={this.props.JoinDateHandler}
                          onFocus={() => {
                            this.props.HandelErrorRemove("Date");
                          }}
                        />
                        <label className="datepicker_label">Joining Date</label>
                      </div>
                    )}
                  </div>
                </div>

                <div className="col-md-12">
                  <div
                    className="form-group buttons_footer_view_users"
                    style={{
                      display: "flex",
                      alignItems: "center",
                      padding: "0px",
                    }}
                  >
                    <button
                      className="btn-black"
                      onClick={this.props.AddUserProfileFun}
                    >
                      {this.props.TaskOwnerHandle?.UserProfileID == 0
                        ? "Save"
                        : "Update"}
                    </button>

                    {this.props.TaskOwnerHandle?.UserProfileID !== 0 && (
                      <button
                        className="btn-black m-3 mb-0 mr-4"
                        type="button"
                        style={{ float: "right" }}
                        onClick={() => this.props.UserProjectsShow("show")}
                        data-toggle="modal"
                        data-target="#TaskOwnerFilter"
                      >
                        <i className="menu-icon fa fa-eye pr-1"></i>
                        View Projects
                      </button>
                    )}
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

export default AddTaskOwnersPopup;
