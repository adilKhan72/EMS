import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import { styling } from "../PopUpMsgModal/stylepopup.css";

class IdleTimeOutModal extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <>
        <Modal
          show={this.props.show}
          onHide={this.props.handleClose}
          backdrop={"static"}
          keyboard={"false"}
          className={styling}
          centered
        >
          <div
            className="modal-content"
            style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>{this.props.Title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{this.props.Msgs}</Modal.Body>
            <Modal.Footer>
              <Button variant="danger" onClick={this.props.handleLogout}>
                Logout
              </Button>
              <Button variant="primary" onClick={this.props.handleClose}>
                Stay
              </Button>
            </Modal.Footer>
          </div>
        </Modal>
      </>
    );
  }
}

export default IdleTimeOutModal;
