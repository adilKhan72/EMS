import React, { Component } from "react";
import { styling } from "./stylepopupBlack.css";
//import { login-modal } from "../Login/login_style.css";

import { Modal, Button, Form } from "react-bootstrap";

class PopUpModalBlack extends Component {
  constructor(props) {
    super(props);
    this.state = {};
    /*     debugger;
    var myElement = document.getElementById("LoginModal");
    // example for addding some-class to the element
    myElement.parentNode.add("login-modal"); */
  }

  render() {
    return (
      <>
        <Modal
          show={this.props.show}
          onHide={this.props.onHide}
          backdrop={"static"}
          keyboard={"false"}
          ClassName={styling}
          centered
          backdropClassName="custom_modal"
          dialogClassName="login-modal"
        >
          <div
            className="modal-content "
            style={{ width: "100%", backgroundColor: "#000", color: "#fff" }}
          >
            <Modal.Header
              style={{ textAlign: "center", border: "0", paddingBottom: "0" }}
            >
              <Modal.Title style={{ textAlign: "center", margin: "auto" }}>
                {this.props.Title}
              </Modal.Title>
            </Modal.Header>
            <Modal.Body style={{ textAlign: "center", marginBottom: "20px" }}>
              {this.props.errorMsgs}
              <br />
              <Button
                className="btn-black"
                onClick={this.props.onHide}
                style={{
                  marginTop: "20px",
                  backgroundColor: "#2984f7",
                  color: "#000",
                  minWidth: "100px",
                  padding: "4px 20px",
                }}
              >
                Ok
              </Button>
            </Modal.Body>
          </div>
        </Modal>
      </>
    );
  }
}

export default PopUpModalBlack;
