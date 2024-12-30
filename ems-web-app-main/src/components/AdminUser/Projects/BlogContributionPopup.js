import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import "./AddProject.css";
import moment from "moment";
class BlogContributionPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    var data = Array.from(this.props.ShowTaskOwner);
    var ShowTaskOwners = null;
    ShowTaskOwners =
      data !== null
        ? data.map((item) => {
            return (
              <button type="button" class="btn-black mr-1 mb-1">
                {item}
              </button>
            );
          })
        : null;
    return (
      <Modal
        show={this.props.BlogContributionShow}
        onHide={this.props.closeBlogContribution}
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
            <h5 className="p-3 pl-4 pb-0">{this.props.Title}</h5>
          </Modal.Header>
          <Modal.Body>
            <>
              <div class="modal-body pt-0">{ShowTaskOwners}</div>
            </>
          </Modal.Body>
        </div>
      </Modal>
    );
  }
}

export default BlogContributionPopup;
