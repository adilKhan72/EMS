import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import TransferList from "./Transferlist/TransferList";

class CreateNotificationPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.CreateNotificationShow}
        onHide={this.props.closeCreateNotification}
        backdrop={"static"}
        keyboard={"false"}
        size="lg"
        aria-labelledby="contained-modal-title-vcenter"
        centered
        dialogClassName="notification_popup"
      >
        <Modal.Header closeButton style={{ border: "none", padding: "0.2rem" }}>
          <h5 className="p-3 pl-4 pb-0">Create New Notification </h5>
        </Modal.Header>
        <Modal.Body>
          <div className="modal-body">
            <div className="row">
              <div className="col-md-12">
                <div
                  className="form-group"
                  style={{
                    alignItems: "center",
                    padding: "0px",
                  }}
                >
                  <label className="col-md-12" style={{ padding: "0" }}>
                    Notification Text
                  </label>
                  <textarea className="form-control col-md-12"></textarea>
                </div>
              </div>

              <div className="col-md-6">
                <div
                  className="form-group"
                  style={{
                    alignItems: "center",
                    padding: "0px",
                  }}
                >
                  <label className="col-md-12" style={{ padding: "0" }}>
                    Notification Start Time
                  </label>
                  <input
                    className="form-control col-md-12"
                    type="text"
                    name=" Notification Start Time"
                  />
                </div>
              </div>
              <div className="col-md-6">
                <div className="form-group ">
                  <div
                    className="customDatePickerWidth col-md-12 "
                    style={{
                      alignItems: "center",
                      padding: "0px",
                      height: "2.3rem",
                    }}
                  >
                    <label className="col-md-12" style={{ padding: "0" }}>
                      Notification End Time
                    </label>
                    <input
                      className="form-control col-md-12"
                      type="text"
                      name=" Notification End Time"
                    />
                  </div>
                </div>
              </div>

              <div className="col-md-12">
                <label className="col-md-12" style={{ padding: "0" }}>
                  Notification Assignees
                </label>
                <TransferList />
              </div>
            </div>
            <hr />
            <button className="btn-black mr-1" type="button">
              Save to Draft
            </button>
            <button className="btn-black mr-1" type="button">
              Publish
            </button>
          </div>
        </Modal.Body>
      </Modal>
    );
  }
}

export default CreateNotificationPopup;
