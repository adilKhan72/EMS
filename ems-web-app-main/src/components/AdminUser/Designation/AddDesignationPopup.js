import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class AddDesignationPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.AddDesignationShow}
        onHide={this.props.closeDesignation}
        backdrop={"static"}
        keyboard={"false"}
        size="md"
        aria-labelledby="contained-modal-title-vcenter"
        centered
      >
        <div className="modal-content">
          <Modal.Header
            closeButton
            style={{ border: "none", padding: "0.2rem" }}
          >
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              {this.props.AddDesignationTitle}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_update_designation">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-12">
                  <div className="form-group">
                    <input
                      className={
                        "form-control " + this.props.errorclass.DesignationName
                      }
                      type="text"
                      name="DesignationName"
                      maxLength={50}
                      value={this.props.AddDesignationHandle.DesignationName}
                      onChange={this.props.AddDesignationHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("DesignationName");
                      }}
                    />
                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Designation Name{" "}
                    </label>
                  </div>
                </div>
                <div className="col-md-12">
                  <div className="form-group styled-select">
                    <select
                      onChange={this.props.AddDesignationHandleChange}
                      className={"form-control " + this.props.errorclass.Status}
                      name="Status"
                      onFocus={() => {
                        this.props.HandelErrorRemove("Status");
                      }}
                    >
                      {this.props.AddDesignationHandle.Status != null ? (
                        this.props.AddDesignationHandle.Status == "Active" ? (
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
                      onClick={this.props.AddDesignationFun}
                    >
                      {this.props.AddDesignationButton}
                    </button>

                    {this.props.AddDesignationTitle == "Update Designation" ? (
                      <button
                        className="btn-black mr-1"
                        onClick={this.props.DeleteRecord}
                      >
                        Delete
                      </button>
                    ) : (
                      ""
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

export default AddDesignationPopup;
