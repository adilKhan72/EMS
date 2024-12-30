import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";

class SearchSetting extends Component {
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
          centered
        >
          <div
            className="modal-content setting_search_popup"
            style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>
                <span className=" text-left">{this.props.Title}</span>
                <span
                  className="clear_btn"
                  onClick={() => {
                    this.props.ClearSearch();
                  }}
                >
                  Clear Search
                </span>
              </Modal.Title>
            </Modal.Header>
            <Modal.Body className="material_style">
              <div className="form-group mb-0 row">
                <input
                  className="form-control"
                  type="text"
                  name="SettingName"
                  value={this.props.SearchSettingNames}
                  onChange={this.props.handleSearchSettingInput}
                  required
                  autoComplete="off"
                />
                <label>Search Setting</label>
              </div>
            </Modal.Body>
            <Modal.Footer style={{ display: "block", textAlign: "left" }}>
              <div className="text-left ">
                <Button
                  className="btn-black"
                  onClick={this.props.SearchSetting}
                >
                  Search
                </Button>
              </div>
            </Modal.Footer>
          </div>
        </Modal>
      </>
    );
  }
}

export default SearchSetting;
