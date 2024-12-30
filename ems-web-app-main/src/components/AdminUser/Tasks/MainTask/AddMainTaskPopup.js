import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import moment from "moment";
import TextField from "@material-ui/core/TextField";
import Select from "@material-ui/core/Select";
import InputLabel from "@material-ui/core/InputLabel";
import FormControl from "@material-ui/core/FormControl";
class AddMainTaskPopup extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <Modal
        show={this.props.AddMainTaskShow}
        onHide={this.props.closeAddSubTask}
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
            <h5 className="p-3 pl-4 pb-0" style={{ fontWeight: "600" }}>
              {this.props.AddMainTaskTitle}
            </h5>
          </Modal.Header>
          <Modal.Body className="ems_update_maintask">
            <div className="modal-body material_style">
              <div className="row">
              <div className="col-md-12">
                  <div
                    className="form-group"
                  >
                    <input
                      className={"form-control "  
                      + this.props.errorclass.MainTaskName
                    }
                    
                      type="text"
                      name="MainTaskName"
                      value={this.props.AddMainTaskHandle.MainTaskName}
                      onChange={this.props.AddMainTaskHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("MainTaskName");
                      }}
                    />
                     <label className="modal_label" style={{width: "auto"}}> MainTask Name </label>
                   
                    {/* <div className="modal_inputs">
                     

                      <TextField
                        label="MainTask Name"
                        variant="outlined"
                        className="form-control"
                        type="text"
                        name="MainTaskName"
                        value={this.props.AddMainTaskHandle.MainTaskName}
                        onChange={this.props.AddMainTaskHandleChange}
                      />
                    </div> */}
                  </div>
                </div>
                <div className="col-md-12">
                  <div className="form-group styled-select">
                     
                     <select
                           onChange={this.props.AddMainTaskHandleChange}
                           className={"form-control "  
                            + this.props.errorclass.Status 
                         } 
                          name="Status"
                          onFocus={() => {
                            this.props.HandelErrorRemove("Status");
                          }}
                         >
                          
                           {this.props.AddMainTaskHandle.Status != null &&
                            this.props.AddMainTaskHandle.Status != ""? (
                             this.props.AddMainTaskHandle.Status == "Active" ? (
                               <option disabled selected hidden>
                                 Active
                               </option>
                             ) : (
                               <option disabled selected hidden>
                                 In-Active
                               </option>
                             )
                           ) 
                           : (
                             <option disabled selected hidden>
                               ---Select---
                             </option>
                           )
                           }
 
                           <option value="Active" name="Status">
                             Active
                           </option>
                           <option value="In-Active" name="Status">
                             In-Active
                           </option>
                        </select>
                         <label className="modal_label" > Status </label>
                  </div>
                <div
                    className="form-group"
                  >
                   
                   

                      {/* <FormControl className="formControl">
                        <InputLabel htmlFor="Status">Status</InputLabel>
                        <Select
                          variant="outlined"
                          native
                          name="Status"
                          onChange={this.props.AddMainTaskHandleChange}
                        >
                          {this.props.AddMainTaskHandle.Status != null ? (
                            this.props.AddMainTaskHandle.Status == "Active" ? (
                              <option disabled selected hidden>
                                Active
                              </option>
                            ) : (
                              <option disabled selected hidden>
                              In-Active
                              </option>
                            )
                          ) : (
                            <option disabled selected hidden>
                              ---Select---
                            </option>
                          )}

                          <option value="Active" name="Status">
                            Active
                          </option>
                          <option value="In-Active" name="Status">
                            In-Active
                          </option>
                        </Select>
                      </FormControl> */}
                  
                  </div>
                </div>
                {/* <div className="col-md-12">
                  <div className="form-group">
                    <input
                      className={"form-control "  + this.props.errorclass.MainTaskName}
                      type="text"
                      name="MainTaskName"
                      value={this.props.AddMainTaskHandle.MainTaskName}
                      onChange={this.props.AddMainTaskHandleChange}
                      required
                      onFocus={() => {
                        this.props.HandelErrorRemove("MainTaskName");
                      }}
                    />
                    <label className="modal_label"> Name </label>
                  </div>
                </div> */}

                {/* <div className="col-md-12">
                  <div className="form-group styled-select">
                    <select className={"form-control "  + this.props.errorclass.DepartmentName} 
                    name="DepartmentName"
                    onFocus={() => {
                      this.props.HandelErrorRemove("DepartmentName");
                    }}
                    >
                      <option>Software Development</option>
                      <option>Software Designing</option>
                      <option>SEO</option>
                    </select>
                    <label className="modal_label">Active</label>
                  </div>
                </div> */}

                <div className="col-md-12">
                  <div
                    className="form-group"
                    style={{
                      display: "flex",
                      alignItems: "center",
                      padding: "0px",
                    }}
                  >
                    <button
                      className="btn-black mr-1"
                      onClick={this.props.AddMainTaskFun}
                    >
                      {this.props.AddMaintaskButton}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </Modal.Body>
        </div>
      </Modal>
    );
  }
}

export default AddMainTaskPopup;
