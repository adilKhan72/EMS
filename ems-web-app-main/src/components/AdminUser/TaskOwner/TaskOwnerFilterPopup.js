import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
class TaskOwnerFilterPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    var DesignationNameList =
      this.props.DesignationRecord == null
        ? ""
        : this.props.DesignationRecord.map((item) => {
            return (
              <option value={item.DesignationName}>
                {item.DesignationName}
              </option>
            );
          });
    var DepartmentList =
      this.props.DepartmentList == null
        ? ""
        : this.props.DepartmentList.map((item) => {
            if (item.IsActive) {
              return <option value={item.ID}>{item.DepartmentName}</option>;
            }
          });
    return (
      <Modal
        show={this.props.TaskOwnerFilterShow}
        onHide={this.props.closeTaskOwnerFilter}
        backdrop={"static"}
        keyboard={"false"}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div className="modal-content">
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "0.2rem" }}
          >
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              Filter
            </h5>
            {/* <span
              class="clear_btn"
              style={{ cursor: "pointer" }}
              onClick={this.props.ClearSearch}
            >
              Clear Search
            </span> */}
          </Modal.Header>
          <Modal.Body className="ems_update_task">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control"
                      type="text"
                      name="Name"
                      value={this.props.TaskOwnerHandle.Name}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
                      required
                      autoComplete="off"
                    />
                    <label>Full Name</label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      className="form-control"
                      required
                      autoComplete="off"
                      name="Department"
                      value={this.props.TaskOwnerHandle.Department}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
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
                      className="form-control"
                      required
                      autoComplete="off"
                      name="Designation"
                      value={this.props.TaskOwnerHandle.Designation}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
                    >
                      <option></option>
                      {DesignationNameList}
                      {/* <option>Jr.Net Developer</option>
                      <option>Project Manager</option> */}
                    </select>
                    <label>Designation</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control"
                      type="email"
                      name="Email"
                      value={this.props.TaskOwnerHandle.Email}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
                      required
                      autoComplete="off"
                    />
                    <label>Email Address</label>
                  </div>
                </div>
                <div className="col-md-6">
                  <div className="form-group styled-select">
                    <select
                      className="form-control"
                      required
                      autoComplete="off"
                      name="AccountType"
                      value={this.props.TaskOwnerHandle.AccountType}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
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
                      className="form-control"
                      required
                      autoComplete="off"
                      name="Status"
                      value={this.props.TaskOwnerHandle.Status}
                      onChange={this.props.TaskOwnerHandleChangeFilter}
                    >
                      <option></option>
                      <option>Active</option>
                      <option>In-Active</option>
                    </select>
                    <label>Status</label>
                  </div>
                </div>

                <div className="col-md-12">
                  <div
                    className="form-group"
                    style={{
                      display: "flex",
                      alignItems: "center",
                      padding: "0px",
                    }}
                  >
                    <button
                      className="btn-black mr-1"
                      onClick={this.props.FilterUserFun}
                    >
                      Search
                    </button>
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

export default TaskOwnerFilterPopup;
