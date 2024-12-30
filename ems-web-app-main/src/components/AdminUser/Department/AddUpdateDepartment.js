import React, { Component } from "react";
import { Modal } from "react-bootstrap";

class AddUpdateDepartment extends Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.show}
        onHide={this.props.onHide}
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
              {this.props.Title}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_update_client">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-6  mt-3">
                  <div className="form-group">
                    <input
                      className={
                        "form-control " + this.props.errorclass.DepartmentName
                      }
                      type="text"
                      name="DepartmentName"
                      value={this.props.DepartmentHandle.DepartmentName}
                      onChange={this.props.AddDepartmentHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("DepartmentName");
                      }}
                      maxLength={50}
                    />
                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Name{" "}
                    </label>
                  </div>
                </div>
                <div className="col-md-6 mt-3">
                  <div className="form-group styled-select">
                    <select
                      //value={this.props.DepartmentHandle.DepartmentIsActive}
                      onChange={this.props.AddDepartmentHandleChange}
                      className={"form-control " + this.props.errorclass.Status}
                      name="DepartmentIsActive"
                      onFocus={() => {
                        this.props.HandelErrorRemove("DepartmentIsActive");
                      }}
                    >
                      {this.props.DepartmentHandle.DepartmentIsActive !=
                      null ? (
                        this.props.DepartmentHandle.DepartmentIsActive ===
                        "Active" ? (
                          <option disabled selected hidden>
                            Active
                          </option>
                        ) : (
                          <option disabled selected hidden>
                            In-Active
                          </option>
                        )
                      ) : (
                        <option disabled selected hidden>
                          ---Select---
                        </option>
                      )}

                      <option value="Active" name="Status">
                        Active
                      </option>
                      <option value="In-Active" name="Status">
                        In-Active
                      </option>
                    </select>
                    <label className="modal_label"> Status </label>
                  </div>
                  <div className="form-group"></div>
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
                      onClick={this.props.AddDepartFun}
                    >
                      {this.props.AddDepartButton}
                    </button>
                    {this.props.DepartmentHandle?.ID > 0 ? (
                      <button
                        className="btn-black mr-1"
                        onClick={this.props.DeleteRecord}
                      >
                        Delete
                      </button>
                    ) : null}
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

export default AddUpdateDepartment;
