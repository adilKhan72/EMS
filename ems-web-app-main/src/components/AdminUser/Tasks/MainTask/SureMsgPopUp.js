import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class SureMsgPopUp extends Component {
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
          <Modal.Body>{this.props.ShowMsgs}</Modal.Body>
          <Modal.Footer>
            <Button className="btn-black" onClick={this.props.onConfrim}>
              Ok
            </Button>
            
          </Modal.Footer>
        </div>
      </Modal>
    );
  }
}

export default SureMsgPopUp;
