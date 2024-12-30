import React, { Component } from "react";
import { styling } from "./stylepopup.css";
import { Modal, Button, Form } from "react-bootstrap";

class PopUpModal extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
      <>
        <Modal
          show={this.props.show}
          onHide={this.props.onHide}
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
            <Modal.Body>{this.props.errorMsgs}</Modal.Body>
            <Modal.Footer>
              <Button className="btn-black" onClick={this.props.onHide}>
                Ok
              </Button>
            </Modal.Footer>
          </div>
        </Modal>
      </>
    );
  }
}

export default PopUpModal;
