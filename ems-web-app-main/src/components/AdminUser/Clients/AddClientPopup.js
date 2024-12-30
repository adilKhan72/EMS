import React, { Component } from "react";
import { Modal } from "react-bootstrap";

class AddClientPopup extends Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.AddClientShow}
        onHide={this.props.closeAddClientBox}
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
              {this.props.AddClientTitle}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_update_client">
            <div className="modal-body material_style">
              <div className="row">
                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className={
                        "form-control " + this.props.errorclass.ClientName
                      }
                      type="text"
                      name="ClientName"
                      value={this.props.AddClientHandle.ClientName}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientName");
                      }}
                      maxLength={50}
                    />
                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Name{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className={
                        "form-control " + this.props.errorclass.ClientEmail
                      }
                      type="text"
                      name="ClientEmail"
                      value={this.props.AddClientHandle.ClientEmail}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientEmail");
                      }}
                      maxLength={40}
                    />
                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Email{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className={
                        "form-control " + this.props.errorclass.ClientContact
                      }
                      name="ClientContact"
                      type="text"
                      value={this.props.AddClientHandle.ClientContact}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientContact");
                      }}
                      maxLength={20}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Contact{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control "
                      name="ClientAddress"
                      type="text"
                      value={this.props.AddClientHandle.ClientAddress}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientAddress");
                      }}
                      maxLength={300}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Address{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control "
                      name="ClientWebsite"
                      type="text"
                      value={this.props.AddClientHandle.ClientWebsite}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientWebsite");
                      }}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Website{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control "
                      name="ClientFacebook"
                      type="text"
                      value={this.props.AddClientHandle.ClientFacebook}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientFacebook");
                      }}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Facebook{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control "
                      name="ClientTwitter"
                      type="text"
                      value={this.props.AddClientHandle.ClientTwitter}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientTwitter");
                      }}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Twitter{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="form-group">
                    <input
                      className="form-control "
                      name="ClientInstagram"
                      type="text"
                      value={this.props.AddClientHandle.ClientInstagram}
                      onChange={this.props.AddClientHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("ClientInstagram");
                      }}
                    />

                    <label className="modal_label" style={{ width: "auto" }}>
                      {" "}
                      Instagram{" "}
                    </label>
                  </div>
                </div>

                <div className="col-md-6 mt-3">
                  <div className="form-group styled-select">
                    <select
                      onChange={this.props.AddClientHandleChange}
                      className={"form-control " + this.props.errorclass.Status}
                      name="Status"
                      onFocus={() => {
                        this.props.HandelErrorRemove("Status");
                      }}
                    >
                      {this.props.AddClientHandle.Status != null ? (
                        this.props.AddClientHandle.Status === "Active" ? (
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
                      onClick={this.props.AddClientFun}
                    >
                      {this.props.AddClientButton}
                    </button>

                    {this.props.AddClientTitle == "Update Client" ? (
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

export default AddClientPopup;
