import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class ShowNotificationPopUp extends Component {
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
        dialogClassName="CustomWidthNotification"
      >
        <Modal.Header
          closeButton={false}
          style={{ borderBottom: "none", paddingBottom: "0" }}
        >
          <Modal.Title className="w-100 text-center">
            {this.props.Title}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body style={{ maxHeight: "60vh", overflowY: "auto" }}>
          {this.props.errorMsgs}
        </Modal.Body>
        <Modal.Footer style={{ borderTop: "none", paddingTop: "0" }}>
          <div className="w-100 text-center ">
            <Button className="btn-black " onClick={this.props.onHide}>
              Ok
            </Button>
          </div>
        </Modal.Footer>
      </Modal>
    );
  }
}

export default ShowNotificationPopUp;
