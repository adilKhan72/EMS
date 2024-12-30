import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import Cookies from "js-cookie";
//import moment from "moment";

class AddSettingModal extends Component {
  constructor(props) {
    super(props);
  }
  /* AddSetting = () => {
    var element = document.getElementById("abc");
    element.classList.add("error_input");
  }; */
  render() {
    //console.log(this.props.errorclass);
    return (
      <>
        <Modal
          show={this.props.show}
          onHide={this.props.onHide}
          backdrop={"static"}
          keyboard={"false"}
          // backdrop={"static"}
          // keyboard={"false"}
          size="lg"
          aria-labelledby="contained-modal-title-vcenter"
          centered
        >
          <div
            className="modal-content"
            style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
          >
            <Modal.Header closeButton>
              <Modal.Title>{this.props.Title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
              <div className="modal-body material_style" id="abc">
                <div className="row">
                  <div className="col-md-6 ">
                    <div className="form-group ">
                      <input
                        className={
                          "form-control " + this.props.errorclass.SettingName
                        }
                        type="text"
                        name="SettingName"
                        value={this.props.SettingNames}
                        onChange={this.props.HandleAddSettingInput}
                        required
                        autoComplete="off"
                        onFocus={() => {
                          this.props.HandelErrorRemove("SettingName");
                        }}
                      />
                      <label>Setting Name</label>
                    </div>
                  </div>

                  <div className="col-md-6 ">
                    <div className="form-group ">
                      <input
                        className={
                          "form-control " + this.props.errorclass.SettingValue
                        }
                        type="text"
                        name="SettingValue"
                        onChange={this.props.HandleAddSettingInput}
                        value={this.props.SettingValues}
                        required
                        autoComplete="off"
                        onFocus={() => {
                          this.props.HandelErrorRemove("SettingValue");
                        }}
                      />
                      <label>Setting Value</label>
                    </div>
                  </div>

                  <div className="col-md-6 mt-2">
                    <div className="form-group styled-select">
                      <select
                        id="select"
                        className={
                          "form-control " + this.props.errorclass.SettingType
                        }
                        name="SettingType"
                        onChange={this.props.HandleAddSettingInput}
                        value={this.props.SettingType}
                        onFocus={() => {
                          this.props.HandelErrorRemove("SettingType");
                        }}
                      >
                        <option value="" disabled selected hidden>
                          ---Select---
                        </option>
                        <option value="Setting A">Setting A</option>
                        <option value="Setting B">Setting B</option>
                        <option value="Setting C">Setting C</option>

                        {""}
                      </select>
                      <label className="modal_label">Setting Type</label>
                    </div>
                  </div>

                  <div className="col-md-6 mt-2">
                    <div className="form-group datepicker_mbl">
                      <div className="customDatePickerWidth ">
                        <label id="lebel" className="datepicker_label">
                          Created Date
                        </label>

                        <DatePicker
                          disabled
                          //dateFormat="dd MMMM yyyy"
                          placeholderText={"DD/MM/YYYY"}
                          selected={this.props.Date}
                          shouldCloseOnSelect={true}
                          className="form-control input-md "
                          // selected={this.props.Date}
                          required
                          autoComplete="off"
                        />
                      </div>
                    </div>
                  </div>

                  <div className="col-md-6 ">
                    <div className="form-group ">
                      <input
                        className={
                          "form-control " +
                          this.props.errorclass.SettingDescription
                        }
                        type="text"
                        name="SettingDescription"
                        onChange={this.props.HandleAddSettingInput}
                        value={this.props.SettingDescription}
                        required
                        autoComplete="off"
                        onFocus={() => {
                          this.props.HandelErrorRemove("SettingDescription");
                        }}
                      />
                      <label>Setting Description</label>
                    </div>
                  </div>

                  {/* <div className="col-md-6">
                    <div className="form-group mb-2">
                      <input
                        className={
                          "form-control " +
                          this.props.errorclass.SettingUserName
                        }
                        type="text"
                        name="SettingUserName"
                        onChange={this.props.HandleAddSettingInput}
                        value={Cookies.get("UserName")}
                        required
                        autoComplete="off"
                        onFocus={() => {
                          this.props.HandelErrorRemove("SettingUserName");
                        }}
                      />
                      <label>Created By</label>
                    </div>
                  </div> */}
                </div>
                <div className="w-100 text-left ">
                  <Button
                    className="btn-black"
                    onClick={() => this.props.AddSetting("addSetting")}
                  >
                    Add
                  </Button>
                </div>
              </div>
            </Modal.Body>
          </div>
        </Modal>
      </>
    );
  }
}

export default AddSettingModal;
