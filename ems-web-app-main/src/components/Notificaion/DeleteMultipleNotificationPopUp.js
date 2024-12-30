import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import "../PopUpMsgModal/stylepopup.css";

class DeleteMultipleNotificationPopUp extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <Modal
        show={this.props.show}
        onHide={this.props.onHide}
        backdrop={"static"}
        keyboard={"false"}
        centered
      >
        <div
          className="modal-content"
          style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header closeButton>
            <Modal.Title>{this.props.Title}</Modal.Title>
          </Modal.Header>
          <Modal.Body>{this.props.errorMsgs}</Modal.Body>
          <Modal.Footer>
            <Button className="btn-black " onClick={this.props.ConfrimDelete}>
              Yes
            </Button>
            <Button className="btn-black" onClick={this.props.onHide}>
              Cancel
            </Button>
          </Modal.Footer>
        </div>
      </Modal>
    );
  }
}

export default DeleteMultipleNotificationPopUp;
